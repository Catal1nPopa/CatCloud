namespace CatCloud.Models.Mail
{
    public class ResponseHelpModel
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
