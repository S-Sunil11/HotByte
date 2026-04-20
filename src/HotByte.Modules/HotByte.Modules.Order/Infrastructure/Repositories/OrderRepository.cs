using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Order.Application.Interfaces;

namespace HotByte.Modules.Order.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context) { _context = context; }

        public async Task AddAsync(Domain.Entities.Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Domain.Entities.Order?> GetByIdAsync(int id)
            => await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<Domain.Entities.Order>> GetByUserIdAsync(int userId)
            => await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<List<Domain.Entities.Order>> GetByRestaurantIdAsync(int restaurantId)
            => await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.RestaurantId == restaurantId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<List<Domain.Entities.Order>> GetByRestaurantIdsAsync(List<int> restaurantIds)
            => await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => restaurantIds.Contains(o.RestaurantId))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<List<Domain.Entities.Order>> GetAllAsync()
            => await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task UpdateAsync(Domain.Entities.Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
