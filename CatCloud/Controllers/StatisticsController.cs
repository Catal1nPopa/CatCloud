using Application.Interfaces;
using CatCloud.Models.Statistics;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController(IStatisticsService statisticsService, IFilesService filesService) : ControllerBase
    {
        private readonly IStatisticsService _statisticsService = statisticsService;
        private readonly IFilesService _fileService = filesService;

        [Authorize]
        [HttpGet("StorageDetails")]
        public async Task<ActionResult<StorageDetailsModel>> GetStorageDetails()
        {
            try
            {
                var storageDetails = await _statisticsService.GetStorageDetails();
                return Ok(storageDetails.Adapt<StorageDetailsModel>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("TableChart")]
        public async Task<ActionResult<UserFilesStatisticsModel>> GetTableChartStatistics()
        {
            try
            {
                var filesStatistics = await _fileService.GetUserFilesStatistics();
                return Ok(filesStatistics.Adapt<List<UserFilesStatisticsModel>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
