
using Application.DTOs.Statistics;

namespace Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<StorageDetailsDTO> GetStorageDetails();
    }
}
