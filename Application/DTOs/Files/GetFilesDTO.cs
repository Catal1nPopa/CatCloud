using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Files
{
    public class GetFilesDTO
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedByUserId { get; set; }
        public IFormFile File { get; set; }
        public string ContentType { get; set; }
    }
}
