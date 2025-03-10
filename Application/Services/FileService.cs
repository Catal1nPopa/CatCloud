using Application.DTOs.Files;
using Application.DTOs.Storage;
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
            await encryptionService.EncryptFileAsync(file, filePath, filesDTO.UploadedByUserId);
            await _fileRepository.UploadFile(filesDTO.Adapt<FileEntity>());
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
                        UploadedAt = file.UploadedAt
                    };

                    FileEncryptionService encryptionService = new FileEncryptionService(_configuration);
                    byte[] decryptedBytes = await encryptionService.DecryptFileAsync(file.FilePath);
                    var memoryStream = new MemoryStream(decryptedBytes);

                    fileRecord.File = new FormFile(memoryStream, 0, memoryStream.Length, null, file.FileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/octet-stream"
                    };

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

        public async Task DeleteFile(Guid fileId, Guid userId)
        {
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

        //public async Task<List<FilesMetadataDTO>> GetUserGroupFilesMetadata(Guid userId)
        //{
        //    var userId = _userProvider.GetUserId();
        //    var files = await _fileRepository.GetUserGroupFilesMetadata(userId);
        //    return files.Adapt<List<>>();
        //}
    }
}
