namespace Application.DTOs.Notification;

public class NotificationDTO
{
    public Guid UserId { get; set; }
    public required string Message { get; set; }
    public DateTime Timestamp { get; set; }
}