using System.ComponentModel.DataAnnotations;
using HotByte.SharedKernel;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Restaurant.Domain.Entities
{
    public class Restaurant : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public RestaurantStatus Status { get; set; } = RestaurantStatus.Active;

        public int OwnerUserId { get; set; }

        public double Rating { get; set; } = 0.0;

        public int TotalRatings { get; set; } = 0;

        // Optional category tag so we can search restaurants by category name (e.g. "Pizza", "Arabian")
        [MaxLength(100)]
        public string? Category { get; set; }
    }
}
