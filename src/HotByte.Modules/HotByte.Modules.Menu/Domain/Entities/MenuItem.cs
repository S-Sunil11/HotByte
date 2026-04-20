using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotByte.SharedKernel;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Menu.Domain.Entities
{
    public class MenuItem : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Ingredients { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? DiscountPrice { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public int RestaurantId { get; set; }

        public MealTime AvailabilityTime { get; set; } = MealTime.AllDay;
        public DietaryType DietaryType { get; set; } = DietaryType.Veg;
        public TasteInfo TasteInfo { get; set; } = TasteInfo.Savory;

        public int CookingTimeMinutes { get; set; } = 30;

        public int? Calories { get; set; }
        public decimal? Fats { get; set; }
        public decimal? Proteins { get; set; }
        public decimal? Carbohydrates { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsOutOfStock { get; set; } = false;

        public virtual MenuCategory? Category { get; set; }
    }
}
