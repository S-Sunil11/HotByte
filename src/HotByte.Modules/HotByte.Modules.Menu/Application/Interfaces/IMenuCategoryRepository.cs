using HotByte.Modules.Menu.Domain.Entities;

namespace HotByte.Modules.Menu.Application.Interfaces
{
    public interface IMenuCategoryRepository
    {
        Task AddAsync(MenuCategory category);
        Task<List<MenuCategory>> GetAllAsync();
        Task<MenuCategory?> GetByIdAsync(int id);
        Task UpdateAsync(MenuCategory category);
        Task DeleteAsync(int id);
    }
}
