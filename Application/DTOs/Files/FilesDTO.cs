namespace Application.DTOs.Files
{
    public class FilesDTO
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedByUserId { get; set; }
        public string ContentType { get; set; }
        public bool ShouldEncrypt { get; set; }
    }
}
