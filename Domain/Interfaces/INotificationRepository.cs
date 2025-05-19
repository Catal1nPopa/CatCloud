using Domain.Entities.Notification;

namespace Domain.Interfaces;

public interface INotificationRepository
{
    Task SaveNotification(NotificationEntity notification);
    Task<List<NotificationEntity>> GetNotifications(Guid userId);
}