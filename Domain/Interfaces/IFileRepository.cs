using Domain.Entities.Files;
using Domain.Entities.UserGroup;

namespace Domain.Interfaces
{
    public interface IFileRepository
    {
        Task UploadFile(FileEntity fileEntity);
        Task ShareFileWithUsers(Guid fileId, List<Guid> userIds);
        Task ShareFileWithGroups(Guid fileId, List<Guid> groupIds);
        Task DeleteFile(Guid fileId, Guid userId);
        Task<List<FileEntity>> GetUserFiles(Guid userId);
        Task<List<FileEntity>> GetFilesSharedWithUser(Guid userId);
        Task<List<FileEntity>> GetFilesSharedWithGroup(Guid groupId);
        Task<FileEntity> GetFileById(Guid fileId, Guid AuthorId);
        Task<List<FilesMetadataEntity>> GetUserFilesMetadata(Guid userId);
        Task<List<FilesMetadataEntity>> GetUserSharedFilesMetadata(Guid userId);
        Task<List<FilesMetadataEntity>> GetUserGroupFilesMetadata(Guid userId);

        Task<List<FilesMetadataEntity>> GetUserOrphanFilesMetadata(Guid userId);
        Task<List<FilesMetadataEntity>> GetUserFolderFilesMetadata(Guid userId, Guid folderId);
        Task<List<GroupFilesMetadata>> GetGroupFilesMetadata(Guid groupId);
    }
}
