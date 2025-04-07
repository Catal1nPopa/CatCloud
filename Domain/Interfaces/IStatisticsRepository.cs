
using Domain.Entities.Statistics;

namespace Domain.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<StatisticsEntity> GetStorageDetails(Guid userId);
    }
}
