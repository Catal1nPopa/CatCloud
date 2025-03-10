using Domain.Entities.Auth;
using Domain.Entities.Files;

namespace Domain.Entities.Folder
{
    public class FolderEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public Guid OwnerId { get; set; }
        public UserEntity Owner { get; set; }

        public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
    }
}
