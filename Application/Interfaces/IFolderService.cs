using Application.DTOs.Folder;

namespace Application.Interfaces
{
    public interface IFolderService
    {
        Task CreateFolder(FolderDTO folder);
        Task LinkFileToFolder(Guid fileId, Guid folderId);
        Task<List<FolderDTO>> GetUserFolders();
        Task DeleteFolder(Guid folderId);
    }
}
