using System;
using System.Collections.Generic;
using Application.DTOs.Files;
using Application.Interfaces;
using Application.Services;
using CatCloud.Models.File;
using CatCloud.Models.User;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Threading.Tasks;
using Application.DTOs.Notification;
using CatCloud.Models.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFilesService filesService, IHubContext<NotificationHub.NotificationHub> notificationHub, INotificationService notificationService) : ControllerBase
    {
        private readonly IFilesService _filesService = filesService;

        [Authorize]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(6L * 1024 * 1024 * 1024)]
        [HttpPost("file")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile userFile, [FromForm] bool shouldEncrypt)
        {
            try
            {
                var fileMetadata = new FileUploadModel(
                    userFile.FileName, DateTime.UtcNow, userFile.Length, userFile.ContentType, shouldEncrypt);
                await _filesService.UploadFiles(userFile, fileMetadata.Adapt<FilesDTO>());
                return Ok(new { Message = $"Fisierul {userFile.FileName} incarcat cu succes" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("file")]
        public async Task<IActionResult> DeleteFile(Guid fileId)
        {
            try
            {
                await _filesService.DeleteFile(fileId);
                return Ok(new { Message = $"Fisierul a fost sters" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("shareUsers")]
        public async Task<IActionResult> ShareFileWithUsers(ShareFileModel shareFileModel)
        {
            try
            {
                await _filesService.ShareFileWithUsers(shareFileModel.fileId, shareFileModel.objectIds);
                foreach (var userId in shareFileModel.objectIds)
                {
                    var notification = new NotificationModel
                    {
                        Message = "Aveți un fișier nou partajat",
                        Timestamp = DateTime.UtcNow,
                        UserId = userId
                    };
                    await notificationHub.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", new { message = notification.Message, timestamp = notification.Timestamp });
                    await notificationService.SaveNotification(notification.Adapt<NotificationDTO>());
                }
                return Ok(new { Message = $"Fisierul a fost partajat cu {shareFileModel.objectIds.Count} utilizatori" });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Authorize]
        [HttpPost("shareGroup")]
        public async Task<IActionResult> ShareFileWithGroups(ShareFileModel shareFileModel)
        {
            try
            {
                await _filesService.ShareFileWithGroups(shareFileModel.fileId, shareFileModel.objectIds);
                return Ok(new { Message = $"Fisierul a fost partajat cu succes" });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // [Authorize]
        // [HttpPost("copyFile")]
        // public async Task<IActionResult> CopyFile([FromBody] CopyFileModel fileModel)
        // {
        //     try
        //     {
        //         await _filesService.CopyFile(fileModel.Adapt<CopyFileDTO>());
        //         return Ok();
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        [Authorize]
        [HttpGet("userFilesMetadata")]
        public async Task<ActionResult<List<FileMetadata>>> GetUserFileMetadata()
        {
            try
            {
                var result = await _filesService.GetUserFilesMetadata();
                return Ok(result.Adapt<List<FileMetadata>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("groupFilesMetadata")]
        public async Task<ActionResult<List<FileMetadata>>> GetGroupFileMetadata(Guid groupId)
        {
            try
            {
                var result = await _filesService.GetGroupFilesMetadata(groupId);
                return Ok(result.Adapt<List<FileMetadata>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("userSharedFilesMetadata")]
        public async Task<ActionResult<List<FileMetadata>>> GetUserSharedFileMetadata()
        {
            try
            {
                var result = await _filesService.GetUserSharedFilesMetadata();
                return Ok(result.Adapt<List<FileMetadata>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("userOrphanFilesMetadata")]
        public async Task<ActionResult<List<FileMetadata>>> GetUserOrphanFileMetadata()
        {
            try
            {
                var result = await _filesService.GetUserOrphanFilesMetadata();
                return Ok(result.Adapt<List<FileMetadata>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("userFolderFilesMetadata")]
        public async Task<ActionResult<List<FileMetadata>>> GetUserFolderFileMetadata(Guid folderId)
        {
            try
            {
                var result = await _filesService.GetUserFolderFilesMetadata(folderId);
                return Ok(result.Adapt<List<FileMetadata>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("fileDownload")]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            try
            {
                var file = await _filesService.DownloadFile(fileId);    
                if (file == null)
                {
                    return NotFound("File not found.");
                }

                var encodedFileName = Uri.EscapeDataString(file.FileName); 
                Response.Headers["Content-Disposition"] = $"attachment; filename*=UTF-8''{encodedFileName}";

                return File(file.bytes, file.ContentType, file.FileName, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("filePreview")]
        public async Task<IActionResult> PreviewFile(Guid fileId)
        {
            try
            {
                var file = await _filesService.DownloadFile(fileId);
                if (file == null)
                {
                    return NotFound("File not found.");
                }

                var encodedFileName = Uri.EscapeDataString(file.FileName);
                Response.Headers["Content-Disposition"] = $"inline; filename*=UTF-8''{encodedFileName}";

                return File(file.bytes, file.ContentType, file.FileName, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error previewing file: {ex.Message}");
            }
        }


        [Authorize]
        [HttpGet("latestUplaodedFiles")]
        public async Task<ActionResult<List<FileMetadata>>> GetLatestUplaodedFiles()
        {
            try
            {
                var result = await _filesService.LatestUploadedFilesMetadata();
                return Ok(result.Adapt<List<FileMetadata>>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}