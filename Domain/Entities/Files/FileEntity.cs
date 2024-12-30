using Domain.Entities.Auth;

namespace Domain.Entities.Files
{
    public class FileEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedByUserId { get; set; }

        public UserEntity UploadedByUser { get; set; }


        public ICollection<FileUserShareEntity> SharedWithUsers { get; set; }
        public ICollection<FileGroupShareEntity> SharedWithGroups { get; set; }
    }
}
