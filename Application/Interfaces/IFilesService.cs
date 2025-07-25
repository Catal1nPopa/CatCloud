﻿using Application.DTOs.Files;
using Application.DTOs.Statistics;
using Application.DTOs.Storage;
using Application.DTOs.UserGroup;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFilesService
    {
        Task UploadFiles(IFormFile file, FilesDTO filesDTO);
        // Task<List<GetFilesDTO>> GetUserFiles(Guid userId);
        // Task<List<GetFilesDTO>> GetFilesSharedWithGroup(Guid groupId);
        // Task<List<GetFilesDTO>> GetFilesSharedWithUser(Guid userId);
        Task DeleteFile(Guid fileId);
        Task ShareFileWithUsers(Guid fileId, List<Guid> userIds);
        Task ShareFileWithGroups(Guid fileId, List<Guid> groupIds);
        Task<List<FilesMetadataDTO>> GetUserFilesMetadata();
        Task<List<FilesMetadataDTO>> GetUserSharedFilesMetadata();
        Task<List<FilesMetadataDTO>> LatestUploadedFilesMetadata();
        //Task<List<FilesMetadataDTO>> GetUserGroupFilesMetadata(Guid userId);
        Task<List<UserFilesStatisticsDTO>> GetUserFilesStatistics();
        Task<List<FilesMetadataDTO>> GetUserOrphanFilesMetadata();
        Task<List<FilesMetadataDTO>> GetUserFolderFilesMetadata(Guid folderId);
        Task<GetFilesDTO> DownloadFile(Guid fileId);
        Task<List<GroupFilesMetadataDTO>> GetGroupFilesMetadata(Guid groupId);
        Task<List<StorageInfoDTO>> GetStoragesInfo();
    }
}
