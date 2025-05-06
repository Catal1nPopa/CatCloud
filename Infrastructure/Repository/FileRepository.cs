using Domain.Entities.Files;
using Domain.Entities.UserGroup;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repository
{
    public class FileRepository(CloudDbContext cloudDbContext) : IFileRepository
    {
        private readonly CloudDbContext _cloudDbContext = cloudDbContext;
        public async Task UploadFile(FileEntity fileEntity)
        {
            try
            {
                _cloudDbContext.Files.Add(fileEntity);
                await _cloudDbContext.SaveChangesAsync();
            }
            catch (PostgresException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch
            {
                throw new Exception($"Eroare la salvare fisier in baza de date: Fisier: {fileEntity.FileName} \n Utilizator: {fileEntity.UploadedByUserId}");
            }
        }

        public async Task ShareFileWithUsers(Guid fileId, List<Guid> userIds)
        {
            var file = await _cloudDbContext.Files.FindAsync(fileId);
            foreach (var userId in userIds)
            {
                var user = await _cloudDbContext.Users.FindAsync(userId);
                var exists = await _cloudDbContext.FileUserShares.AnyAsync(fu => fu.FileId == fileId && fu.UserId == userId);
                if (!exists)
                {
                    var userShare = new FileUserShareEntity
                    {
                        FileId = fileId,
                        UserId = userId,
                        SharedAt = DateTime.UtcNow,
                        File = file,
                        User = user
                    };
                    _cloudDbContext.FileUserShares.Add(userShare);

                    //adauga file catre user nou
                    //var copyFile = file;
                    //copyFile.UploadedByUserId = userId;
                    //copyFile.Owner = user;
                    //copyFile.SharedWithUsers = null;
                    //copyFile.SharedWithGroups = null;
                    //copyFile.

                    //_cloudDbContext.Files.Add(fileEntity);
                }
                else
                    throw new Exception("Fisierul deja a fost partajat");
            }
            await _cloudDbContext.SaveChangesAsync();
        }

        public async Task ShareFileWithGroups(Guid fileId, List<Guid> groupIds)
        {
            var file = await _cloudDbContext.Files.FindAsync(fileId);
            foreach (var groupId in groupIds)
            {
                var group = await _cloudDbContext.Groups.FindAsync(groupId);
                var exists = await _cloudDbContext.FileGroupShares.AnyAsync(fg => fg.FileId == fileId && fg.GroupId == groupId);
                if (!exists)
                {
                    var groupShare = new FileGroupShareEntity
                    {
                        FileId = fileId,
                        GroupId = groupId,
                        SharedAt = DateTime.UtcNow,
                        File = file,
                        Group = group

                    };
                    _cloudDbContext.FileGroupShares.Add(groupShare);
                }
            }

            await _cloudDbContext.SaveChangesAsync();
            //throw new NotImplementedException();
        }

        public async Task<List<FileEntity>> GetUserFiles(Guid userId)
        {
            try
            {
                return await _cloudDbContext.Files
                    .Where(file => file.UploadedByUserId == userId)
                    .ToListAsync();
            }
            catch
            {
                throw new Exception($"Eroare la obtinerea fisierelor pentru utilizatorul: {userId}");
            }
        }

        public async Task<List<FileEntity>> GetFilesSharedWithUser(Guid userId)
        {
            try
            {
                var fileIds = await _cloudDbContext.FileUserShares
                    .Where(fg => fg.UserId == userId)
                    .Select(fg => fg.FileId)
                    .ToListAsync();

                var files = await _cloudDbContext.Files
                    .Where(f => fileIds.Contains(f.Id))
                    .ToListAsync();

                return files;
            }
            catch
            {
                throw new Exception($"Eroare la obtinerea fisierelor partajate pentru utilizatorul: {userId}");
            }
        }

        public async Task<List<FileEntity>> GetFilesSharedWithGroup(Guid groupId)
        {
            //try
            //{
            //    var fileIds = await _cloudDbContext.FileGroupShares
            //        .Where(fg => fg.GroupId == groupId)
            //        .Select(fg => fg.FileId)
            //        .ToListAsync();

            //    var files = await _cloudDbContext.Files
            //        .Where(f => fileIds.Contains(f.Id))
            //        .ToListAsync();

            //    return files;
            //}
            //catch
            //{
            //    throw new Exception($"Eroare la obtinerea fisierelor partajate pentru grupul: {groupId}");
            //}
            throw new NotImplementedException();
        }

        public async Task DeleteFile(Guid fileId, Guid userId)
        {
            try
            {

                var file = await _cloudDbContext.Files.FirstOrDefaultAsync(f => f.Id == fileId && f.UploadedByUserId == userId);
                if (file == null) throw new FileNotFoundException("File not found or unauthorized.");

                if (File.Exists(file.FilePath))
                {
                    File.Delete(file.FilePath);
                }
                
                _cloudDbContext.Files.Remove(file);
                _cloudDbContext.FileUserShares.RemoveRange(_cloudDbContext.FileUserShares.Where(fu => fu.FileId == fileId));
                //_cloudDbContext.FileGroupShares.RemoveRange(_cloudDbContext.FileGroupShares.Where(fg => fg.FileId == fileId));

                await _cloudDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la sterge fisier {fileId} de catre utilizatorul {userId}");
            }
        }

        public async Task<FileEntity> GetFileById(Guid fileId, Guid authorId)
        {
            try
            {
                return await _cloudDbContext.Files.FirstOrDefaultAsync(id => id.Id == fileId); //&& id.UploadedByUserId == authorId
            }
            catch (Exception ex)
            {
                throw new Exception($"Fisierul nu a fost gasit");
            }
        }

        public async Task<List<FilesMetadataEntity>> GetUserFilesMetadata(Guid userId)
        {
            var res = await _cloudDbContext.Files
            .Where(f => f.UploadedByUserId == userId)
            .Include(f => f.UserEntities)
            .Include(f => f.SharedWithUsers).ThenInclude(f => f.User)
            .Include(f => f.SharedWithGroups).ThenInclude(swu => swu.Group)
            .Select(f => new FilesMetadataEntity
            {
                Id = f.Id,
                FileName = f.FileName,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt,
                ContentType = f.ContentType,
                ShouldEncrypt = f.ShouldEncrypt,
                SharedWithUsers = f.SharedWithUsers.Select(swu => swu.User.Email).ToList(),
                SharedWithGroups = f.SharedWithGroups.Select(swg => swg.Group.Name).ToList()
            })
            .ToListAsync();

            return res;
        }

        public async Task<List<FilesMetadataEntity>> LatestUploadedFilesMetadata(Guid userId)
        {
            var res = await _cloudDbContext.Files
                .Where(f => f.UploadedByUserId == userId)
                .OrderByDescending(f => f.UploadedAt)
                .Take(10)
                .Select(f => new FilesMetadataEntity
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt,
                    ContentType = f.ContentType,
                })
                .ToListAsync();
            return res;
        }


        public async Task<List<FilesMetadataEntity>> GetUserOrphanFilesMetadata(Guid userId)
        {
            var res = await _cloudDbContext.Files
                .Where(file => file.FolderId == null && file.UploadedByUserId == userId)
                .Include(f => f.SharedWithUsers).ThenInclude(f => f.User)
                .Include(f => f.SharedWithGroups).ThenInclude(swu => swu.Group)
                .Select(f => new FilesMetadataEntity
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt,
                    ContentType = f.ContentType,
                    ShouldEncrypt = f.ShouldEncrypt,
                    SharedWithUsers = f.SharedWithUsers.Select(swu => swu.User.Email).ToList(),
                    SharedWithGroups = f.SharedWithGroups.Select(swg => swg.Group.Name).ToList()
                })
                .ToListAsync();
            return res;
        }

        public async Task<List<GroupFilesMetadata>> GetGroupFilesMetadata(Guid groupId)
        {
            var files = await _cloudDbContext.Files
                //.Include(f => f.Owner) //cine a incarcat
                .Include(f => f.GroupEntities) // cu ce grupuri e partajat
                .ThenInclude(g => g.UserEntities) //utilizatorii grupului
                .Where(f => f.GroupEntities.Any(g => g.Id == groupId))
                .Select(file => new GroupFilesMetadata
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    FileSize = file.FileSize,
                    UploadedAt = file.UploadedAt,
                    ContentType = file.ContentType,
                    OwnerFile = file.Owner.Email,
                    //Users = file.UserEntities.Select(email => email.Email).ToList(),  
                })
                .ToListAsync();

            return files;
        }

        public async Task<List<FilesMetadataEntity>> GetUserFolderFilesMetadata(Guid userId, Guid folderId)
        {
            var res = await _cloudDbContext.Files
                .Where(file => file.FolderId == folderId && file.UploadedByUserId == userId)
                .Include(f => f.SharedWithUsers).ThenInclude(f => f.User)
                .Include(f => f.SharedWithGroups).ThenInclude(swu => swu.Group)
                .Select(f => new FilesMetadataEntity
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt,
                    ContentType = f.ContentType,
                    ShouldEncrypt = f.ShouldEncrypt,
                    SharedWithUsers = f.SharedWithUsers.Select(swu => swu.User.Email).ToList(),
                    SharedWithGroups = f.SharedWithGroups.Select(swg => swg.Group.Name).ToList()
                })
                .ToListAsync();
            return res;
        }

        public async Task<List<FilesMetadataEntity>> GetUserGroupFilesMetadata(Guid userId)
        {
            var res = await _cloudDbContext.Files
            .Where(f => f.UploadedByUserId == userId)
            .Include(f => f.UserEntities)
            .Include(f => f.SharedWithUsers)
            .Include(f => f.SharedWithGroups)
            //.Include(f => f.SharedWithGroups)
            .ToListAsync();

            List<FilesMetadataEntity> resEntity = new List<FilesMetadataEntity>();
            return resEntity;
        }

        public async Task<List<FilesMetadataEntity>> GetUserSharedFilesMetadata(Guid userId)
        {
            try
            {
                var sharedFiles = await _cloudDbContext.FileUserShares
                    .Where(fu => fu.UserId == userId)
                    .Include(fu => fu.File)
                    .ThenInclude(f => f.SharedWithUsers)
                    .Include(fu => fu.File)
                    .ThenInclude(f => f.SharedWithGroups)
                    .Select(fu => new FilesMetadataEntity
                    {
                        Id = fu.File.Id,
                        FileName = fu.File.FileName,
                        FileSize = fu.File.FileSize,
                        UploadedAt = fu.File.UploadedAt,
                        ContentType = fu.File.ContentType,
                        ShouldEncrypt = fu.File.ShouldEncrypt,
                        SharedWithUsers = fu.File.SharedWithUsers.Select(swu => swu.User.Email).ToList(),
                        SharedWithGroups = fu.File.SharedWithGroups.Select(swg => swg.Group.Name).ToList()
                    })
                    .ToListAsync();

                return sharedFiles;
            }
            catch
            {
                throw new Exception($"Eroare la obținerea metadatelor fișierelor partajate pentru utilizatorul: {userId}");
            }
        }
    }
}
