using HotByte.Modules.Menu.Application.DTOs;
using HotByte.Modules.Menu.Domain.Entities;

namespace HotByte.Modules.Menu.Application.Mappings
{
    public static class MenuMapper
    {
        public static MenuCategoryDto ToDto(MenuCategory e)
            => new(e.Id, e.Name, e.Description, e.ImageUrl, e.IsActive, e.CreatedAt);

        public static List<MenuCategoryDto> ToDtoList(List<MenuCategory> entities)
            => entities.Select(ToDto).ToList();

        public static MenuCategory ToEntity(CreateMenuCategoryDto dto)
            => new()
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

        public static MenuItemDto ToDto(MenuItem e)
            => new(
                e.Id,
                e.Name,
                e.Description,
                e.Ingredients,
                e.Price,
                e.DiscountPrice,
                e.ImageUrl,
                e.CategoryId,
                e.Category?.Name,
                e.RestaurantId,
                e.AvailabilityTime,
                e.DietaryType,
                e.TasteInfo,
                e.CookingTimeMinutes,
                e.Calories,
                e.Fats,
                e.Proteins,
                e.Carbohydrates,
                e.IsAvailable,
                e.IsOutOfStock,
                e.CreatedAt);

        public static List<MenuItemDto> ToDtoList(List<MenuItem> entities)
            => entities.Select(ToDto).ToList();

        public static MenuItem ToEntity(CreateMenuItemDto dto)
            => new()
            {
                Name = dto.Name,
                Description = dto.Description,
                Ingredients = dto.Ingredients,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                RestaurantId = dto.RestaurantId,
                AvailabilityTime = dto.AvailabilityTime,
                DietaryType = dto.DietaryType,
                TasteInfo = dto.TasteInfo,
                CookingTimeMinutes = dto.CookingTimeMinutes,
                Calories = dto.Calories,
                Fats = dto.Fats,
                Proteins = dto.Proteins,
                Carbohydrates = dto.Carbohydrates,
                CreatedAt = DateTime.UtcNow
            };
    }
}
