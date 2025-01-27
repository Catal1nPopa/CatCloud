using Domain.Entities.Files;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach (var userId in userIds)
            {
                var exists = await _cloudDbContext.FileUserShares.AnyAsync(fu => fu.FileId == fileId && fu.UserId == userId);
                if (!exists)
                {
                    var userShare = new FileUserShareEntity
                    {
                        FileId = fileId,
                        UserId = userId,
                        SharedAt = DateTime.UtcNow
                    };
                    _cloudDbContext.FileUserShares.Add(userShare);
                }
            }
            await _cloudDbContext.SaveChangesAsync();
        }

        public async Task ShareFileWithGroups(Guid fileId, List<Guid> groupIds)
        {
            foreach (var groupId in groupIds)
            {
                var exists = await _cloudDbContext.FileGroupShares.AnyAsync(fg => fg.FileId == fileId && fg.GroupId == groupId);
                if (!exists)
                {
                    var groupShare = new FileGroupShareEntity
                    {
                        FileId = fileId,
                        GroupId = groupId,
                        SharedAt = DateTime.UtcNow
                    };
                    _cloudDbContext.FileGroupShares.Add(groupShare);
                }
            }

            await _cloudDbContext.SaveChangesAsync();
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
            try
            {
                var fileIds = await _cloudDbContext.FileGroupShares
                    .Where(fg => fg.GroupId == groupId)
                    .Select(fg => fg.FileId)
                    .ToListAsync();

                var files = await _cloudDbContext.Files
                    .Where(f => fileIds.Contains(f.Id))
                    .ToListAsync();

                return files;
            }
            catch
            {
                throw new Exception($"Eroare la obtinerea fisierelor partajate pentru grupul: {groupId}");
            }
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
                _cloudDbContext.FileGroupShares.RemoveRange(_cloudDbContext.FileGroupShares.Where(fg => fg.FileId == fileId));

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
                return await _cloudDbContext.Files.FirstOrDefaultAsync(id => id.Id == fileId && id.UploadedByUserId == authorId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fisierul nu a fost gasit");
            }
        }
    }
}
