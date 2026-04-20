using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.Modules.Menu.Domain.Entities;

namespace HotByte.Modules.Menu.Infrastructure.Repositories
{
    public class MenuCategoryRepository : IMenuCategoryRepository
    {
        private readonly MenuDbContext _context;
        public MenuCategoryRepository(MenuDbContext context) { _context = context; }

        public async Task AddAsync(MenuCategory category)
        {
            _context.MenuCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MenuCategory>> GetAllAsync()
            => await _context.MenuCategories.ToListAsync();

        public async Task<MenuCategory?> GetByIdAsync(int id)
            => await _context.MenuCategories.FindAsync(id);

        public async Task UpdateAsync(MenuCategory category)
        {
            _context.MenuCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.MenuCategories.FindAsync(id);
            if (entity != null)
            {
                _context.MenuCategories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
