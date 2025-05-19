using Application.DTOs.Notification;

namespace Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDTO>> GetNotifications();
    Task SaveNotification(NotificationDTO notification);
}