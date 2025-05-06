using System.Net;
using System.Net.Mail;
using Application.DTOs.Files;
using Application.DTOs.Statistics;
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
            var storagePath = await GetStoragePath(filesDTO.FileSize);
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
            var fileSize = await encryptionService.EncryptFileAsync(file, filePath, filesDTO.UploadedByUserId, filesDTO.UploadedAt);
            filesDTO.FileSize = fileSize;
            if (await _authRepository.DecreaseAvailableSize(fileSize, userId))
            {
                await _fileRepository.UploadFile(filesDTO.Adapt<FileEntity>());
            }
        }

        // public async Task<List<GetFilesDTO>> GetUserFiles(Guid userId)
        // {
        //     //var userFilesPath = await _fileRepository.GetUserFiles(userId);
        //     //var files = await GetDecryptedFiles(userFilesPath);
        //     //return files;
        //     throw new NotImplementedException();
        // }

        // public async Task CopyFile(CopyFileDTO fileDTO)
        // {
        //     var fileData = await _fileRepository.GetFileById(fileDTO.fileId, fileDTO.AuthorId);
        //     var fileEntity = fileData;
        //     var fileRecord = new GetFilesDTO { };
        //     if (File.Exists(fileData.FilePath))
        //     {
        //         FileEncryptionService encryptionService = new FileEncryptionService(_configuration);
        //         byte[] decryptedBytes = await encryptionService.DecryptFileAsync(fileEntity.FilePath, fileEntity.UploadedAt);
        //         var memoryStream = new MemoryStream(decryptedBytes);
        //
        //         fileRecord.File = new FormFile(memoryStream, 0, memoryStream.Length, null, fileData.FileName)
        //         {
        //             Headers = new HeaderDictionary(),
        //             ContentType = "application/octet-stream"
        //         };
        //     }
        //
        //     var user = await _authRepository.GetUserById(fileDTO.UserId);
        //     //fileEntity.UploadedByUser = user;
        //     fileEntity.UploadedByUserId = fileDTO.UserId;
        //     fileEntity.UploadedAt = DateTime.UtcNow;
        //
        //     await UploadFiles(fileRecord.File, fileEntity.Adapt<FilesDTO>());
        //     throw new NotImplementedException();
        // }

        // public async Task<List<GetFilesDTO>> GetFilesSharedWithGroup(Guid groupId)
        // {
        //     throw new NotImplementedException();
        //     var groupFilesPath = await _fileRepository.GetFilesSharedWithGroup(groupId);
        //     var files = await GetDecryptedFiles(groupFilesPath);
        //     return files;
        // }
        // public async Task<List<GetFilesDTO>> GetFilesSharedWithUser(Guid userId)
        // {
        //     var userFilesPath = await _fileRepository.GetFilesSharedWithUser(userId);
        //     var files = null;await GetDecryptedFiles(userFilesPath);
        //     return files;
        //     throw new NotImplementedException();
        // }
        private async Task<GetFilesDTO> GetDecryptedFiles(FileEntity file)
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
                    byte[] decryptedBytes = await encryptionService.DecryptFileAsync(file.FilePath, file.UploadedAt);
                    fileRecord.bytes = decryptedBytes;
                    return fileRecord;
                }
            return null;
        }
        private async Task<string> GetStoragePath(long fileSize)
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
                await SendEmail("misterco2002@gmail.com", "Storage error", $"{ex.Message}");
                throw new Exception($"Nu este disponibila memorie {ex.Message}");
            }
            throw new Exception($"Eroare la accesare memorie");
        }

        public async Task<List<StorageInfoDTO>> GetStoragesInfo()
        {
            List<StorageInfoDTO> storageInfoDTOs = new List<StorageInfoDTO>();
            try
            {
                foreach (var driveLetter in _storageSettings.Value.Storages)
                {
                    var driveInfo = new DriveInfo(driveLetter);
                    long totalSizeMB = driveInfo.TotalSize / (1024 * 1024);
                    long availableSpaceMB = driveInfo.TotalFreeSpace / (1024 * 1024);
                    StorageInfoDTO storageInfo = new StorageInfoDTO(
                        driveInfo.RootDirectory.FullName,
                        driveInfo.IsReady,
                        totalSizeMB,
                        availableSpaceMB
                    );
                    storageInfoDTOs.Add(storageInfo);
                }
                return await Task.FromResult(storageInfoDTOs);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
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

        public async Task<List<UserFilesStatisticsDTO>> GetUserFilesStatistics()
        {
            var userId = _userProvider.GetUserId();
            List<FilesMetadataEntity> files = await _fileRepository.GetUserFilesMetadata(userId);
            foreach (var file in files)
            {
                if (file.UploadedAt.Kind == DateTimeKind.Utc)
                {
                    file.UploadedAt = file.UploadedAt.ToLocalTime();
                }
            }
            return files.Adapt<List<UserFilesStatisticsDTO>>();
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
            var fileDecrypted = await GetDecryptedFiles(file);

            if (fileDecrypted == null || !File.Exists(file.FilePath))
            {
                throw new Exception("File not found.");
            }
            return fileDecrypted;
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

        public async Task<List<FilesMetadataDTO>> LatestUploadedFilesMetadata()
        {
            try
            {
                Guid userId = _userProvider.GetUserId();
                var files = await _fileRepository.LatestUploadedFilesMetadata(userId);
                return files.Adapt<List<FilesMetadataDTO>>();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        //public async Task<List<FilesMetadataDTO>> GetUserGroupFilesMetadata(Guid userId)
        //{
        //    var userId = _userProvider.GetUserId();
        //    var files = await _fileRepository.GetUserGroupFilesMetadata(userId);
        //    return files.Adapt<List<>>();
        //}
        
        private async Task SendEmail(string emailToReceive, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("misterco2002@gmail.com", "yvqs lfrj osaa ygqo");

                    using (var message = new MailMessage(
                               from: new MailAddress("misterco2002@gmail.com","CatStorage"),
                               to: new MailAddress(emailToReceive)))
                    {
                        message.Subject = $"Cat Storage | {subject}!";
                        message.Body = body;
                        message.IsBodyHtml = true;

                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }
    }
}
