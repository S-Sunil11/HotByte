using HotByte.Modules.Order.Application.DTOs;

namespace HotByte.Modules.Order.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, int orderId, string type, string message);
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int requesterUserId);
    }
}
