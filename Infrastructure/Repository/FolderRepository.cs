using Domain.Entities.Folder;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class FolderRepository(CloudDbContext cloudDbContext) : IFolderRepository
    {
        private readonly CloudDbContext _cloudDbContext = cloudDbContext;
        public async Task CreateFolder(FolderEntity folder)
        {
            try
            {
                _cloudDbContext.Folders.Add(folder);
                await _cloudDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la crare folder {folder.Name}, EXCEPTIE {ex.Message}");
            }
        }

        public async Task LinkFileToFolder(Guid fileId, Guid folderId, Guid userId)
        {
            try
            {
                var file = _cloudDbContext.Files.FirstOrDefault(f => f.Id == fileId);
                var folder = _cloudDbContext.Folders.FirstOrDefault(f => f.Id == folderId);
                if (folder.OwnerId != userId)
                {
                    throw new Exception($"Nu aveti acces la acest directoriu. userId : {userId}, folderId: {folderId}");
                }
                if (file != null && folder != null)
                {
                    file.FolderId = folderId;
                    await _cloudDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la atribuire fisier {fileId} in directoriul {folderId}, {ex.Message}");
            }
        }

        public async Task DeleteFolder(Guid folderId, Guid userId)
        {
            try
            {
                var folder = await _cloudDbContext.Folders.FirstOrDefaultAsync(g => g.Id == folderId);
                if (folder != null && folder.OwnerId == userId)
                {
                    _cloudDbContext.Folders.Remove(folder);
                    await _cloudDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Directoriul nu a fost găsit sau nu aveti acces.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la sterge directoriu {folderId}, {ex.Message}");
            }
        }

        public async Task<List<FolderEntity>> GetUserFolders(Guid userId)
        {
            try
            {
                var folders = await _cloudDbContext.Folders
                            .Where(f => f.OwnerId == userId).ToListAsync();
                return folders;
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la afisare directorii pentru utilizatorul {userId}, {ex.Message}");
            }
        }
    }
}
