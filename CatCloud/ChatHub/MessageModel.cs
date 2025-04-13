namespace CatCloud.ChatHub
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string ChatRoomId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
