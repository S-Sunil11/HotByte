using HotByte.Modules.Menu.Application.DTOs;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Menu.Application.Interfaces
{
    public interface IMenuItemService
    {
        Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto, int userId, string role);
        Task<List<MenuItemDto>> GetAllMenuItemsAsync();
        Task<MenuItemDto?> GetMenuItemByIdAsync(int id);
        Task<List<MenuItemDto>> GetMenuItemsByRestaurantAsync(int restaurantId);
        Task<List<MenuItemDto>> GetMenuItemsByCategoryAsync(int categoryId);
        Task<List<MenuItemDto>> SearchMenuItemsAsync(string keyword);
        Task<List<MenuItemDto>> FilterMenuItemsAsync(int? restaurantId, int? categoryId, DietaryType? dietaryType, decimal? minPrice, decimal? maxPrice);
        Task<bool> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto, int userId, string role);
        Task<bool> DeleteMenuItemAsync(int id, int userId, string role);
        Task<bool> ToggleOutOfStockAsync(int id, int userId, string role);
    }
}
