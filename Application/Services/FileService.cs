using Application.DTOs.Files;
using Application.DTOs.Storage;
using Application.DTOs.UserGroup;
using Application.Interfaces;
using Domain.Entities.Files;
using Domain.Interfaces;
using Helper.Cryptography;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class FileService(IFileRepository fileRepository,
        IAuthRepository authRepository,
        IOptions<StorageSettings> storageSettings,
        IConfiguration configuration,
        IUserProvider userProvider) : IFilesService
    {
        private readonly IFileRepository _fileRepository = fileRepository;
        private readonly IOptions<StorageSettings> _storageSettings = storageSettings;
        private readonly IConfiguration _configuration = configuration;
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IUserProvider _userProvider = userProvider;

        public async Task UploadFiles(IFormFile file, FilesDTO filesDTO)
        {
            var userId = _userProvider.GetUserId();
            var user = await _authRepository.GetUserById(userId);
            //long availableStorageInBytes = (long)(user.AvailableStorage * 1073741824);
            if (user.AvailableStorage <= filesDTO.FileSize)
            {
                throw new Exception($"Nu aveti memorie disponibila pentru acest fisier");
            }
            filesDTO.UploadedByUserId = userId;
            var storagePath = GetStoragePath(filesDTO.FileSize);
            var userFolderPath = Path.Combine(storagePath, "files", filesDTO.UploadedByUserId.ToString());
            if (!File.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);
            }
            var filePath = Path.Combine(userFolderPath, filesDTO.FileName);
            int count = 1;

            while (File.Exists(filePath))
            {
                filePath = Path.Combine(userFolderPath, $"{Path.GetFileNameWithoutExtension(filesDTO.FileName)} ({count++}){Path.GetExtension(filesDTO.FileName)}");
            }

            filesDTO.FilePath = filePath;
            FileEncryptionService encryptionService = new FileEncryptionService(_configuration);
            var fileSize = await encryptionService.EncryptFileAsync(file, filePath, filesDTO.UploadedByUserId);
            filesDTO.FileSize = fileSize;
            if (await _authRepository.DecreaseAvailableSize(fileSize, userId))
            {
                await _fileRepository.UploadFile(filesDTO.Adapt<FileEntity>());
            }
        }

        public async Task<List<GetFilesDTO>> GetUserFiles(Guid userId)
        {
            var userFilesPath = await _fileRepository.GetUserFiles(userId);
            var files = await GetDecryptedFiles(userFilesPath);
            return files;
        }

        public async Task CopyFile(CopyFileDTO fileDTO)
        {
            var fileData = await _fileRepository.GetFileById(fileDTO.fileId, fileDTO.AuthorId);
            var fileEntity = fileData;
            var fileRecord = new GetFilesDTO { };
            if (File.Exists(fileData.FilePath))
            {
                FileEncryptionService encryptionService = new FileEncryptionService(_configuration);
                byte[] decryptedBytes = await encryptionService.DecryptFileAsync(fileEntity.FilePath);
                var memoryStream = new MemoryStream(decryptedBytes);

                fileRecord.File = new FormFile(memoryStream, 0, memoryStream.Length, null, fileData.FileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                };
            }

            var user = await _authRepository.GetUserById(fileDTO.UserId);
            //fileEntity.UploadedByUser = user;
            fileEntity.UploadedByUserId = fileDTO.UserId;
            fileEntity.UploadedAt = DateTime.UtcNow;

            await UploadFiles(fileRecord.File, fileEntity.Adapt<FilesDTO>());
        }

        public async Task<List<GetFilesDTO>> GetFilesSharedWithGroup(Guid groupId)
        {
            var groupFilesPath = await _fileRepository.GetFilesSharedWithGroup(groupId);
            var files = await GetDecryptedFiles(groupFilesPath);
            return files;
        }
        public async Task<List<GetFilesDTO>> GetFilesSharedWithUser(Guid userId)
        {
            var userFilesPath = await _fileRepository.GetFilesSharedWithUser(userId);
            var files = await GetDecryptedFiles(userFilesPath);
            return files;
        }
        private async Task<List<GetFilesDTO>> GetDecryptedFiles(List<FileEntity> fileEntities)
        {
            var fileRecords = new List<GetFilesDTO>();
            foreach (var file in fileEntities)
            {
                if (File.Exists(file.FilePath))
                {
                    var fileRecord = new GetFilesDTO
                    {
                        UploadedByUserId = file.UploadedByUserId,
                        FileName = file.FileName,
                        FileSize = file.FileSize,
                        UploadedAt = file.UploadedAt,
                        ContentType = file.ContentType
                    };

                    FileEncryptionService encryptionService = new FileEncryptionService(_configuration);
                    byte[] decryptedBytes = await encryptionService.DecryptFileAsync(file.FilePath);
                    //var memoryStream = new MemoryStream(decryptedBytes);

                    //fileRecord.File = new FormFile(memoryStream, 0, memoryStream.Length, null, file.FileName)
                    //{
                    //    Headers = new HeaderDictionary(),
                    //    ContentType = "application/octet-stream"
                    //};

                    string decryptedFilePath = Path.Combine(Path.GetTempPath(), file.FileName);
                    await File.WriteAllBytesAsync(decryptedFilePath, decryptedBytes);

                    fileRecord.File = new FormFile(new FileStream(decryptedFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), 0, new FileInfo(decryptedFilePath).Length, null, file.FileName);


                    fileRecords.Add(fileRecord);
                }
            }
            return fileRecords;
        }
        private string GetStoragePath(long fileSize)
        {
            try
            {
                foreach (var driveLetter in _storageSettings.Value.Storages)
                {
                    var driveInfo = new DriveInfo(driveLetter);
                    if (driveInfo.IsReady && driveInfo.AvailableFreeSpace > fileSize)
                    {
                        return driveInfo.RootDirectory.FullName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Nu este disponibila memorie {ex.Message}");
            }
            throw new Exception($"Eroare la accesare memorie");
        }

        public async Task DeleteFile(Guid fileId)
        {
            var userId = _userProvider.GetUserId();
            var file = await _fileRepository.GetFileById(fileId, userId);
            await _authRepository.IncreaseAvailableSize(file.FileSize, userId);
            await _fileRepository.DeleteFile(fileId, userId);
        }

        public async Task ShareFileWithUsers(Guid fileId, List<Guid> userIds)
        {
            await _fileRepository.ShareFileWithUsers(fileId, userIds);
        }

        public async Task ShareFileWithGroups(Guid fileId, List<Guid> groupIds)
        {
            await _fileRepository.ShareFileWithGroups(fileId, groupIds);
        }

        public async Task<List<FilesMetadataDTO>> GetUserFilesMetadata()
        {
            var userId = _userProvider.GetUserId();
            var files = await _fileRepository.GetUserFilesMetadata(userId);
            return files.Adapt<List<FilesMetadataDTO>>();
        }

        public async Task<List<FilesMetadataDTO>> GetUserSharedFilesMetadata()
        {
            var userId = _userProvider.GetUserId();
            var files = await _fileRepository.GetUserSharedFilesMetadata(userId);
            return files.Adapt<List<FilesMetadataDTO>>();
        }

        public async Task<List<FilesMetadataDTO>> GetUserOrphanFilesMetadata()
        {
            var userId = _userProvider.GetUserId();
            var files = await _fileRepository.GetUserOrphanFilesMetadata(userId);
            return files.Adapt<List<FilesMetadataDTO>>();
        }
        public async Task<List<FilesMetadataDTO>> GetUserFolderFilesMetadata(Guid folderId)
        {
            var userId = _userProvider.GetUserId();
            var files = await _fileRepository.GetUserFolderFilesMetadata(userId, folderId);
            return files.Adapt<List<FilesMetadataDTO>>();
        }

        public async Task<GetFilesDTO> DownloadFile(Guid fileId)
        {
            var userId = _userProvider.GetUserId();
            var file = await _fileRepository.GetFileById(fileId, userId);
            List<FileEntity> filesToDecrypt = new List<FileEntity>();
            filesToDecrypt.Add(file);
            var fileDecrypted = await GetDecryptedFiles(filesToDecrypt);

            if (fileDecrypted == null || !File.Exists(file.FilePath))
            {
                throw new Exception("File not found.");
            }
            return fileDecrypted.First();
        }

        public async Task<List<GroupFilesMetadataDTO>> GetGroupFilesMetadata(Guid groupId)
        {
            try
            {
                var files =  await _fileRepository.GetGroupFilesMetadata(groupId);
                return files.Adapt<List<GroupFilesMetadataDTO>>();
            }catch (Exception ex)
            {
                throw new Exception();
            }
        }

        //public async Task<List<FilesMetadataDTO>> GetUserGroupFilesMetadata(Guid userId)
        //{
        //    var userId = _userProvider.GetUserId();
        //    var files = await _fileRepository.GetUserGroupFilesMetadata(userId);
        //    return files.Adapt<List<>>();
        //}
    }
}
