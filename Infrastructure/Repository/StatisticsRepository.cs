
using Domain.Entities.Statistics;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class StatisticsRepository(CloudDbContext dbContext) : IStatisticsRepository
    {
        private readonly CloudDbContext _dbContext = dbContext;
        public async Task<StatisticsEntity> GetStorageDetails(Guid userId)
        {
            try
            {

                var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);

                var imageContentTypes = new[] { "image/png", "image/jpg", "image/jpeg", "image/webp" };
                var documentContentTypes = new[]
                {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };

                // VIDEO
                var videoQuery = _dbContext.Files
                    .Where(f => f.UploadedByUserId == userId && f.ContentType == "video/mp4");
                var videoFilesCount = await videoQuery.CountAsync();
                var videoFilesSize = await videoQuery.SumAsync(f => f.FileSize);

                // IMAGES
                var imageQuery = _dbContext.Files
                    .Where(f => f.UploadedByUserId == userId && imageContentTypes.Contains(f.ContentType));
                var imageFilesCount = await imageQuery.CountAsync();
                var imageFilesSize = await imageQuery.SumAsync(f => f.FileSize);

                // DOCUMENTS
                var documentQuery = _dbContext.Files
                    .Where(f => f.UploadedByUserId == userId && documentContentTypes.Contains(f.ContentType));
                var documentFilesCount = await documentQuery.CountAsync();
                var documentFilesSize = await documentQuery.SumAsync(f => f.FileSize);

                // OTHER
                var allTypes = new List<string> { "video/mp4" }
                    .Concat(imageContentTypes)
                    .Concat(documentContentTypes)
                    .ToList();

                var otherQuery = _dbContext.Files
                    .Where(f => f.UploadedByUserId == userId && !allTypes.Contains(f.ContentType));
                var otherFilesCount = await otherQuery.CountAsync();
                var otherFilesSize = await otherQuery.SumAsync(f => f.FileSize);

                return new StatisticsEntity
                {
                    totalMemory = Math.Round(user.TotalStorage / (1024.0 * 1024.0), 2),
                    availableMemory = Math.Round(user.AvailableStorage / (1024.0 * 1024.0), 2),
                    documents = documentFilesCount,
                    documentsSize = Math.Round(documentFilesSize / (1024.0 * 1024.0), 2),
                    videos = videoFilesCount,
                    videosSize = Math.Round(videoFilesSize / (1024.0 * 1024.0), 2),
                    images = imageFilesCount,
                    imagesSize = Math.Round(imageFilesSize / (1024.0 * 1024.0), 2),
                    other = otherFilesCount,
                    otherSize = Math.Round(otherFilesSize / (1024.0 * 1024.0), 2),
                };

            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la afisare statistica, {ex.Message}");
            }
        }

    }
}
