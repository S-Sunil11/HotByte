using System.ComponentModel.DataAnnotations;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Menu.Application.DTOs
{
    public record MenuCategoryDto(
        int Id,
        string Name,
        string? Description,
        string? ImageUrl,
        bool IsActive,
        DateTime CreatedAt);

    public record CreateMenuCategoryDto(
        [Required][MaxLength(100)] string Name,
        [MaxLength(300)] string? Description,
        [MaxLength(500)] string? ImageUrl);

    public record UpdateMenuCategoryDto(
        [MaxLength(100)] string? Name,
        [MaxLength(300)] string? Description,
        [MaxLength(500)] string? ImageUrl,
        bool? IsActive);

    public record MenuItemDto(
        int Id,
        string Name,
        string? Description,
        string? Ingredients,
        decimal Price,
        decimal? DiscountPrice,
        string? ImageUrl,
        int CategoryId,
        string? CategoryName,
        int RestaurantId,
        MealTime AvailabilityTime,
        DietaryType DietaryType,
        TasteInfo TasteInfo,
        int CookingTimeMinutes,
        int? Calories,
        decimal? Fats,
        decimal? Proteins,
        decimal? Carbohydrates,
        bool IsAvailable,
        bool IsOutOfStock,
        DateTime CreatedAt);

    public record CreateMenuItemDto(
        [Required][MaxLength(200)] string Name,
        [MaxLength(1000)] string? Description,
        [MaxLength(500)] string? Ingredients,
        [Required][Range(0.01, 1000000)] decimal Price,
        decimal? DiscountPrice,
        [MaxLength(500)] string? ImageUrl,
        [Required] int CategoryId,
        [Required] int RestaurantId,
        MealTime AvailabilityTime = MealTime.AllDay,
        DietaryType DietaryType = DietaryType.Veg,
        TasteInfo TasteInfo = TasteInfo.Savory,
        int CookingTimeMinutes = 30,
        int? Calories = null,
        decimal? Fats = null,
        decimal? Proteins = null,
        decimal? Carbohydrates = null);

    public record UpdateMenuItemDto(
        [MaxLength(200)] string? Name,
        [MaxLength(1000)] string? Description,
        [MaxLength(500)] string? Ingredients,
        decimal? Price,
        decimal? DiscountPrice,
        [MaxLength(500)] string? ImageUrl,
        int? CategoryId,
        MealTime? AvailabilityTime,
        DietaryType? DietaryType,
        TasteInfo? TasteInfo,
        int? CookingTimeMinutes,
        int? Calories,
        decimal? Fats,
        decimal? Proteins,
        decimal? Carbohydrates,
        bool? IsAvailable,
        bool? IsOutOfStock);
}
