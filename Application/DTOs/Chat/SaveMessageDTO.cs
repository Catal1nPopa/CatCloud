

namespace Application.DTOs.Chat
{
    public class SaveMessageDTO
    {
        public string Message { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
