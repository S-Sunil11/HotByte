using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Domain.Entities;

namespace HotByte.Modules.Order.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly OrderDbContext _context;

        public NotificationRepository(OrderDbContext context) { _context = context; }

        public async Task AddAsync(OrderNotification notification)
        {
            _context.OrderNotifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderNotification?> GetByIdAsync(int id)
            => await _context.OrderNotifications.FindAsync(id);

        public async Task<List<OrderNotification>> GetByUserIdAsync(int userId)
            => await _context.OrderNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var n = await _context.OrderNotifications.FindAsync(notificationId);
            if (n == null) return false;
            n.IsRead = true;
            n.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
