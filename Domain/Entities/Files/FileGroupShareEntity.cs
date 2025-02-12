using Domain.Entities.UserGroup;

namespace Domain.Entities.Files
{
    public class FileGroupShareEntity
    {
        public Guid FileId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime SharedAt { get; set; }

        public FileEntity File { get; set; }
        public GroupEntity Group { get; set; }
    }
}
