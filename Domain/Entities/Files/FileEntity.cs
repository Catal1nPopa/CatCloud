using Domain.Entities.Auth;
using Domain.Entities.UserGroup;

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
        public string ContentType { get; set; }

        //public UserEntity UploadedByUser { get; set; }


        public ICollection<FileUserShareEntity> SharedWithUsers { get; set; }
        public ICollection<FileGroupShareEntity> SharedWithGroups { get; set; }
        public ICollection<UserEntity> UserEntities { get; set; }
        public ICollection<GroupEntity> GroupEntities { get; set; }
    }
}
