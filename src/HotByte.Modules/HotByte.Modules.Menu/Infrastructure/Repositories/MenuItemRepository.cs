using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.Modules.Menu.Domain.Entities;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Menu.Infrastructure.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly MenuDbContext _context;
        public MenuItemRepository(MenuDbContext context) { _context = context; }

        public async Task AddAsync(MenuItem item)
        {
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MenuItem>> GetAllAsync()
            => await _context.MenuItems.Include(m => m.Category).ToListAsync();

        public async Task<MenuItem?> GetByIdAsync(int id)
            => await _context.MenuItems.Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<List<MenuItem>> GetByRestaurantIdAsync(int restaurantId)
            => await _context.MenuItems.Include(m => m.Category)
                .Where(m => m.RestaurantId == restaurantId).ToListAsync();

        public async Task<List<MenuItem>> GetByCategoryIdAsync(int categoryId)
            => await _context.MenuItems.Include(m => m.Category)
                .Where(m => m.CategoryId == categoryId).ToListAsync();

        public async Task<List<MenuItem>> SearchAsync(string keyword)
            => await _context.MenuItems.Include(m => m.Category)
                .Where(m => m.Name.Contains(keyword) ||
                    (m.Description != null && m.Description.Contains(keyword)) ||
                    (m.Ingredients != null && m.Ingredients.Contains(keyword)))
                .ToListAsync();

        public async Task<List<MenuItem>> FilterAsync(
            int? restaurantId, int? categoryId, DietaryType? dietaryType, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.MenuItems.Include(m => m.Category).AsQueryable();

            if (restaurantId.HasValue)
                query = query.Where(m => m.RestaurantId == restaurantId.Value);
            if (categoryId.HasValue)
                query = query.Where(m => m.CategoryId == categoryId.Value);
            if (dietaryType.HasValue)
                query = query.Where(m => m.DietaryType == dietaryType.Value);
            if (minPrice.HasValue)
                query = query.Where(m => m.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(m => m.Price <= maxPrice.Value);

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.MenuItems.FindAsync(id);
            if (entity != null)
            {
                _context.MenuItems.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
