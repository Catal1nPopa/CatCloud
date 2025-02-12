using Application.DTOs.Files;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFilesService
    {
        Task UploadFiles(IFormFile file, FilesDTO filesDTO);
        Task<List<GetFilesDTO>> GetUserFiles(Guid userId);
        Task<List<GetFilesDTO>> GetFilesSharedWithGroup(Guid groupId);
        Task<List<GetFilesDTO>> GetFilesSharedWithUser(Guid userId);
        Task DeleteFile(Guid fileId, Guid userId);
        Task ShareFileWithUsers(Guid fileId, List<Guid> userIds);
        Task ShareFileWithGroups(Guid fileId, List<Guid> groupIds);
        Task CopyFile(CopyFileDTO fileDTO);
        Task<List<FilesMetadataDTO>> GetUserFilesMetadata();
        //Task<List<FilesMetadataDTO>> GetUserGroupFilesMetadata(Guid userId);
    }
}
