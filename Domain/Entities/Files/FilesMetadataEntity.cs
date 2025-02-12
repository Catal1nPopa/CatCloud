namespace Domain.Entities.Files
{
    public class FilesMetadataEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ContentType { get; set; }
        public List<string> SharedWithUsers { get; set; }
        public List<string> SharedWithGroups { get; set; }
    }
}
