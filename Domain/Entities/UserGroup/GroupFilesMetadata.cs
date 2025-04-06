
namespace Domain.Entities.UserGroup
{
    public class GroupFilesMetadata
    {
        public Guid Id { get; set; }
        public string OwnerFile { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ContentType { get; set; }
        //public List<string> Users { get; set; }
    }
}
