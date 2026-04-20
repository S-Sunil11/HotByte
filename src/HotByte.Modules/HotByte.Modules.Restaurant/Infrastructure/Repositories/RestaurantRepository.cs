using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Restaurant.Application.Interfaces;

namespace HotByte.Modules.Restaurant.Infrastructure.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Domain.Entities.Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Domain.Entities.Restaurant>> GetAllAsync()
            => await _context.Restaurants.ToListAsync();

        public async Task<Domain.Entities.Restaurant?> GetByIdAsync(int id)
            => await _context.Restaurants.FindAsync(id);

        public async Task<List<Domain.Entities.Restaurant>> GetByOwnerIdAsync(int ownerUserId)
            => await _context.Restaurants
                .Where(r => r.OwnerUserId == ownerUserId)
                .ToListAsync();

        public async Task<List<Domain.Entities.Restaurant>> SearchAsync(string? name, string? category)
        {
            var query = _context.Restaurants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(r => r.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(r => r.Category != null && r.Category.Contains(category));

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(Domain.Entities.Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Restaurants.FindAsync(id);
            if (entity != null)
            {
                _context.Restaurants.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
