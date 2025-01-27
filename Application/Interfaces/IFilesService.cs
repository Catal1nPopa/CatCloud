using Application.DTOs.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
