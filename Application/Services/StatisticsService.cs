
using Application.DTOs.Statistics;
using Application.Interfaces;
using Domain.Interfaces;
using Mapster;

namespace Application.Services
{
    public class StatisticsService(IStatisticsRepository statisticsRepository, IUserProvider userProvider) : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository = statisticsRepository;
        private readonly IUserProvider _userProvider = userProvider;
        public async Task<StorageDetailsDTO> GetStorageDetails()
        {
            Guid userId = _userProvider.GetUserId();
            var storageDetailsEntity = await _statisticsRepository.GetStorageDetails(userId);
            storageDetailsEntity.usedMemory = Math.Round(storageDetailsEntity.totalMemory - storageDetailsEntity.availableMemory, 2);
            return storageDetailsEntity.Adapt<StorageDetailsDTO>();
        }   
    }
}
