
using Domain.Entities.Auth;

namespace Domain.Entities.Mail
{
    public class ResponseHelpEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string UserMail { get; set; }
        public string Username { get; set; }
    }
}
