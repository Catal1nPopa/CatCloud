
namespace Application.DTOs.UserGroup
{
    public class GroupFilesMetadataDTO
    {
        public Guid Id { get; set; }
        public string OwnerFile { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ContentType { get; set; }
    }
}
