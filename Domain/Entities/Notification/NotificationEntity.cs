using Domain.Entities.Auth;

namespace Domain.Entities.Notification;

public class NotificationEntity
{
    public Guid Id { get; set; }
    public string Message { get; set; } = null!;
    public DateTime Timestamp { get; set; }

    public Guid? UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}