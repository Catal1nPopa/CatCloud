using Domain.Entities.Folder;

namespace Domain.Interfaces
{
    public interface IFolderRepository
    {
        Task CreateFolder(FolderEntity folder);
        Task LinkFileToFolder(Guid fileId, Guid folderId, Guid userId);
        Task DeleteFolder(Guid folderId, Guid userId);
        Task<List<FolderEntity>> GetUserFolders(Guid userId);
    }
}
