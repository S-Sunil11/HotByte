using HotByte.Modules.Menu.Application.DTOs;
using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.Modules.Menu.Application.Mappings;

namespace HotByte.Modules.Menu.Application.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IMenuCategoryRepository _repository;

        public MenuCategoryService(IMenuCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<MenuCategoryDto> CreateCategoryAsync(CreateMenuCategoryDto dto)
        {
            var entity = MenuMapper.ToEntity(dto);
            await _repository.AddAsync(entity);
            return MenuMapper.ToDto(entity);
        }

        public async Task<List<MenuCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetAllAsync();
            return MenuMapper.ToDtoList(categories);
        }

        public async Task<MenuCategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category == null ? null : MenuMapper.ToDto(category);
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateMenuCategoryDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.ImageUrl != null) entity.ImageUrl = dto.ImageUrl;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
