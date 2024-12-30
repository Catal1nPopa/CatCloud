using Domain.Entities.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
