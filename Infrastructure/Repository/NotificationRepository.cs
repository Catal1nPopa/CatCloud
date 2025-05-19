using Domain.Entities.Notification;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class NotificationRepository(CloudDbContext _dbContext) : INotificationRepository
{
    public async Task SaveNotification(NotificationEntity notification)
    {
        try
        {
            _dbContext.Add(notification);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message}");
        }
    }

    public async Task<List<NotificationEntity>> GetNotifications(Guid userId)
    {
        try
        {
            var notifications = await _dbContext.Notifications
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
            return notifications;
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message}");
        }
    }
}