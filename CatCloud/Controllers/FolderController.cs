using Application.DTOs.Folder;
using Application.Interfaces;
using CatCloud.Models.Folder;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController(IFolderService folderService) : ControllerBase
    {
        private readonly IFolderService _folderService = folderService;

        [Authorize]
        [HttpPost("UserFolders")]
        public async Task<IActionResult> CreateFolder([FromBody] FolderModel folder)
        {
            folder.CreatedDate = DateTime.UtcNow;
            await _folderService.CreateFolder(folder.Adapt<FolderDTO>());
            return Ok();
        }

        [Authorize]
        [HttpPost("LinkFileToFolder")]
        public async Task<IActionResult> LinkFileToFolder(Guid fileId, Guid folderId)
        {
            await _folderService.LinkFileToFolder(fileId, folderId);
            return Ok();
        }

        [Authorize]
        [HttpGet("UserFolders")]
        public async Task<ActionResult<List<FolderModel>>> GetUserFolders()
        {
            var response = await _folderService.GetUserFolders();
            return Ok(response.Adapt<List<FolderModel>>());
        }

        [Authorize]
        [HttpDelete("UserFolders")]
        public async Task<IActionResult> DeleteUserFolders(Guid folderId)
        {
            await _folderService.DeleteFolder(folderId);
            return Ok();
        }
    }
}
