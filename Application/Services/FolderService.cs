using Application.DTOs.Folder;
using Application.Interfaces;
using Domain.Entities.Folder;
using Domain.Interfaces;
using Mapster;

namespace Application.Services
{
    public class FolderService(IUserProvider userProvider, IFolderRepository folderRepository) : IFolderService
    {
        private readonly IFolderRepository _folderRepository = folderRepository;
        private readonly IUserProvider _userProvider = userProvider;
        public async Task CreateFolder(FolderDTO folder)
        {
            var userId = _userProvider.GetUserId();
            folder.OwnerId = userId;
            await _folderRepository.CreateFolder(folder.Adapt<FolderEntity>());
        }

        public async Task DeleteFolder(Guid folderId)
        {
            var userId = _userProvider.GetUserId();
            await _folderRepository.DeleteFolder(folderId, userId);
        }

        public async Task<List<FolderDTO>> GetUserFolders()
        {
            var userId = _userProvider.GetUserId();
            var response = await _folderRepository.GetUserFolders(userId);
            return response.Adapt<List<FolderDTO>>();
        }

        public async Task LinkFileToFolder(Guid fileId, Guid folderId)
        {
            var userId = _userProvider.GetUserId();
            await _folderRepository.LinkFileToFolder(fileId, folderId, userId);
        }
    }
}
