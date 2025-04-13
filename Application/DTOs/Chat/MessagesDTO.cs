
namespace Application.DTOs.Chat
{
    public class MessagesDTO
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public Guid GroupId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
