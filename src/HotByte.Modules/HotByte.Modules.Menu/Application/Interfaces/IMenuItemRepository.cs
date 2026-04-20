using HotByte.Modules.Menu.Domain.Entities;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Menu.Application.Interfaces
{
    public interface IMenuItemRepository
    {
        Task AddAsync(MenuItem item);
        Task<List<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(int id);
        Task<List<MenuItem>> GetByRestaurantIdAsync(int restaurantId);
        Task<List<MenuItem>> GetByCategoryIdAsync(int categoryId);
        Task<List<MenuItem>> SearchAsync(string keyword);
        Task<List<MenuItem>> FilterAsync(int? restaurantId, int? categoryId, DietaryType? dietaryType, decimal? minPrice, decimal? maxPrice);
        Task UpdateAsync(MenuItem item);
        Task DeleteAsync(int id);
    }
}
