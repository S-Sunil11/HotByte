using HotByte.Modules.Menu.Application.DTOs;
using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.Modules.Menu.Application.Mappings;
using HotByte.SharedKernel.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Menu.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _repository;
        private readonly IRestaurantPublicService _restaurantService;

        public MenuItemService(
            IMenuItemRepository repository,
            IRestaurantPublicService restaurantService)
        {
            _repository = repository;
            _restaurantService = restaurantService;
        }

        // Requirement 6: ownership check - owner of restaurant 3 cannot manage items in restaurant 4
        private async Task EnsureOwnershipAsync(int restaurantId, int userId, string role)
        {
            if (role == "Admin") return;
            var owns = await _restaurantService.IsOwnerOfRestaurantAsync(userId, restaurantId);
            if (!owns)
                throw new UnauthorizedAccessException($"You do not own restaurant {restaurantId}.");
        }

        public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto, int userId, string role)
        {
            await EnsureOwnershipAsync(dto.RestaurantId, userId, role);

            var entity = MenuMapper.ToEntity(dto);
            await _repository.AddAsync(entity);
            return MenuMapper.ToDto(entity);
        }

        public async Task<List<MenuItemDto>> GetAllMenuItemsAsync()
            => MenuMapper.ToDtoList(await _repository.GetAllAsync());

        public async Task<MenuItemDto?> GetMenuItemByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MenuMapper.ToDto(item);
        }

        public async Task<List<MenuItemDto>> GetMenuItemsByRestaurantAsync(int restaurantId)
            => MenuMapper.ToDtoList(await _repository.GetByRestaurantIdAsync(restaurantId));

        public async Task<List<MenuItemDto>> GetMenuItemsByCategoryAsync(int categoryId)
            => MenuMapper.ToDtoList(await _repository.GetByCategoryIdAsync(categoryId));

        public async Task<List<MenuItemDto>> SearchMenuItemsAsync(string keyword)
            => MenuMapper.ToDtoList(await _repository.SearchAsync(keyword));

        public async Task<List<MenuItemDto>> FilterMenuItemsAsync(
            int? restaurantId, int? categoryId, DietaryType? dietaryType, decimal? minPrice, decimal? maxPrice)
            => MenuMapper.ToDtoList(await _repository.FilterAsync(restaurantId, categoryId, dietaryType, minPrice, maxPrice));

        public async Task<bool> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto, int userId, string role)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            await EnsureOwnershipAsync(entity.RestaurantId, userId, role);

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.Ingredients != null) entity.Ingredients = dto.Ingredients;
            if (dto.Price.HasValue) entity.Price = dto.Price.Value;
            if (dto.DiscountPrice.HasValue) entity.DiscountPrice = dto.DiscountPrice.Value;
            if (dto.ImageUrl != null) entity.ImageUrl = dto.ImageUrl;
            if (dto.CategoryId.HasValue) entity.CategoryId = dto.CategoryId.Value;
            if (dto.AvailabilityTime.HasValue) entity.AvailabilityTime = dto.AvailabilityTime.Value;
            if (dto.DietaryType.HasValue) entity.DietaryType = dto.DietaryType.Value;
            if (dto.TasteInfo.HasValue) entity.TasteInfo = dto.TasteInfo.Value;
            if (dto.CookingTimeMinutes.HasValue) entity.CookingTimeMinutes = dto.CookingTimeMinutes.Value;
            if (dto.Calories.HasValue) entity.Calories = dto.Calories.Value;
            if (dto.Fats.HasValue) entity.Fats = dto.Fats.Value;
            if (dto.Proteins.HasValue) entity.Proteins = dto.Proteins.Value;
            if (dto.Carbohydrates.HasValue) entity.Carbohydrates = dto.Carbohydrates.Value;
            if (dto.IsAvailable.HasValue) entity.IsAvailable = dto.IsAvailable.Value;
            if (dto.IsOutOfStock.HasValue) entity.IsOutOfStock = dto.IsOutOfStock.Value;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteMenuItemAsync(int id, int userId, string role)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            await EnsureOwnershipAsync(entity.RestaurantId, userId, role);
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ToggleOutOfStockAsync(int id, int userId, string role)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            await EnsureOwnershipAsync(entity.RestaurantId, userId, role);
            entity.IsOutOfStock = !entity.IsOutOfStock;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
