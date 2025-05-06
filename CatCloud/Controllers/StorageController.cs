using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using CatCloud.Models.Storage;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController(IFilesService filesService) : ControllerBase
    {
        private readonly IFilesService _filesService = filesService;
        [HttpGet("storageInfo")]
        public async Task<IActionResult> GetStorageInfo()
        {
            try
            {
                var storageInfo = await _filesService.GetStoragesInfo();
                return Ok(storageInfo.Adapt<List<StorageInfoModel>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
