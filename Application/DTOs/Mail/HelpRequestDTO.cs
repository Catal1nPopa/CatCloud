

namespace Application.DTOs.Mail
{
    public class HelpRequestDTO
    {
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
