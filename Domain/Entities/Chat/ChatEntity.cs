
using Domain.Entities.Auth;
using Domain.Entities.UserGroup;

namespace Domain.Entities.Chat
{
    public class ChatEntity
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public Guid? UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public Guid? GroupId { get; set; }
        public GroupEntity Group { get; set; } = null!;
    }
}
