namespace CatCloud.Models.Notification;

public class NotificationModel
{
    public string Message { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public Guid? UserId { get; set; }
}