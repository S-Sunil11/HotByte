using HotByte.Modules.Menu.Application.DTOs;

namespace HotByte.Modules.Menu.Application.Interfaces
{
    public interface IMenuCategoryService
    {
        Task<MenuCategoryDto> CreateCategoryAsync(CreateMenuCategoryDto dto);
        Task<List<MenuCategoryDto>> GetAllCategoriesAsync();
        Task<MenuCategoryDto?> GetCategoryByIdAsync(int id);
        Task<bool> UpdateCategoryAsync(int id, UpdateMenuCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
