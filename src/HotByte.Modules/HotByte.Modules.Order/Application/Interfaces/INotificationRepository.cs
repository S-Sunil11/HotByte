using HotByte.Modules.Order.Domain.Entities;

namespace HotByte.Modules.Order.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(OrderNotification notification);
        Task<OrderNotification?> GetByIdAsync(int id);
        Task<List<OrderNotification>> GetByUserIdAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
    }
}
