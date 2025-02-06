using Domain.Entities.Auth;

namespace Domain.Entities.Files
{
    public class FileUserShareEntity
    {
        public Guid FileId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SharedAt { get; set; }

        public FileEntity File { get; set; }
        public UserEntity User { get; set; }
    }
}
