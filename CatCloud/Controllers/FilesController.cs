using Application.DTOs.Files;
using Application.Interfaces;
using CatCloud.Models.File;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace CatCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFilesService filesService) : ControllerBase
    {
        private readonly IFilesService _filesService = filesService;

        [HttpPost("file")]
        public async Task<IActionResult> UploadFile(IFormFile userFile, Guid userId)
        {
            try
            {
                var fileMetadata = new FileUploadModel(
                    userFile.FileName, DateTime.UtcNow, userFile.Length, userId);
                await _filesService.UploadFiles(userFile, fileMetadata.Adapt<FilesDTO>());
                return Ok(new { Message = $"Fisierul {userFile.FileName} incarcat cu succes" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("file")]
        public async Task<IActionResult> DeleteFile(Guid fileId, Guid userId)
        {
            try
            {
                await _filesService.DeleteFile(fileId, userId);
                return Ok(new { Message = $"Fisierul {fileId} sters cu succes" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("file")]
        public async Task<IActionResult> DownloadAllUserFiles(Guid userId)
        {
            var files = await _filesService.GetUserFiles(userId);

            if (files == null || !files.Any())
            {
                return NotFound("Nu au fost găsite fișiere pentru acest utilizator.");
            }

            var memoryStream = new MemoryStream();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileRecord in files)
                {
                    if (fileRecord.File != null)
                    {
                        var zipEntry = archive.CreateEntry(fileRecord.FileName, CompressionLevel.Fastest);

                        using (var entryStream = zipEntry.Open())
                        using (var fileStream = fileRecord.File.OpenReadStream())
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }
            }
            memoryStream.Position = 0;
            return File(memoryStream, "application/zip", $"User_{userId}_Files.zip");
        }

        [HttpPost("shareUsers")]
        public async Task<IActionResult> ShareFileWithUsers(ShareFileModel shareFileModel)
        {
            try
            {
                await _filesService.ShareFileWithUsers(shareFileModel.fileId, shareFileModel.objectIds);
                return Ok(new { Message = $"Fisierul {shareFileModel.fileId} a fost partajat cu utilizatorii: {shareFileModel.objectIds}" });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("shareUsers")]
        public async Task<IActionResult> GetFilesSharedWithUser(Guid userId)
        {
            try
            {
                var files = await _filesService.GetFilesSharedWithUser(userId);
                if (files == null || !files.Any())
                {
                    return NotFound("Nu au fost găsite fișiere pentru acest utilizator.");
                }

                var memoryStream = new MemoryStream();

                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var fileRecord in files)
                    {
                        if (fileRecord.File != null)
                        {
                            var zipEntry = archive.CreateEntry(fileRecord.FileName, CompressionLevel.Fastest);

                            using (var entryStream = zipEntry.Open())
                            using (var fileStream = fileRecord.File.OpenReadStream())
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }
                memoryStream.Position = 0;
                return File(memoryStream, "application/zip", $"User_{userId}_Files.zip");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("shareGroup")]
        public async Task<IActionResult> ShareFileWithGroups(ShareFileModel shareFileModel)
        {
            try
            {
                await _filesService.ShareFileWithGroups(shareFileModel.fileId, shareFileModel.objectIds);
                return Ok(new { Message = $"Fisierul {shareFileModel.fileId} a fost partajat cu utilizatorii: {shareFileModel.objectIds}" });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("shareGroup")]
        public async Task<IActionResult> GetFilesSharedWithGroup(Guid groupId)
        {
            try
            {
                var files = await _filesService.GetFilesSharedWithGroup(groupId);
                if (files == null || !files.Any())
                {
                    return NotFound("Nu au fost găsite fișiere pentru acest utilizator.");
                }

                var memoryStream = new MemoryStream();

                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var fileRecord in files)
                    {
                        if (fileRecord.File != null)
                        {
                            var zipEntry = archive.CreateEntry(fileRecord.FileName, CompressionLevel.Fastest);

                            using (var entryStream = zipEntry.Open())
                            using (var fileStream = fileRecord.File.OpenReadStream())
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }
                memoryStream.Position = 0;
                return File(memoryStream, "application/zip", $"Group_{groupId}_Files.zip");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("copyFile")]
        public async Task<IActionResult> CopyFile([FromBody] CopyFileModel fileModel)
        {
            try
            {
                await _filesService.CopyFile(fileModel.Adapt<CopyFileDTO>());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}