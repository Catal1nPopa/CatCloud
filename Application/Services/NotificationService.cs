using Application.DTOs.Notification;
using Application.Interfaces;
using Domain.Entities.Notification;
using Domain.Interfaces;
using Mapster;

namespace Application.Services;

public class NotificationService(INotificationRepository notificationRepository, IUserProvider userProvider) : INotificationService
{
    public async Task<List<NotificationDTO>> GetNotifications()
    {
        var userId = userProvider.GetUserId();
        var notifications = await notificationRepository.GetNotifications(userId);
        return notifications.Adapt<List<NotificationDTO>>();
    }

    public async Task SaveNotification(NotificationDTO notification)
    {
        await notificationRepository.SaveNotification(notification.Adapt<NotificationEntity>());
    }
}