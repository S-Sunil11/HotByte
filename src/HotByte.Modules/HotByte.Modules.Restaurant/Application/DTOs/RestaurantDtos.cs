using System.ComponentModel.DataAnnotations;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Restaurant.Application.DTOs
{
    public record RestaurantDto(
        int Id,
        string Name,
        string? Description,
        string Location,
        string? ContactNumber,
        string? Email,
        string? ImageUrl,
        RestaurantStatus Status,
        int OwnerUserId,
        double Rating,
        int TotalRatings,
        string? Category,
        DateTime CreatedAt);

    // Requirement 8: Admin creates restaurant AND owner account in ONE call
    public record CreateRestaurantWithOwnerDto(
        [Required][MaxLength(200)] string Name,
        [MaxLength(500)] string? Description,
        [Required][MaxLength(500)] string Location,
        [MaxLength(20)] string? ContactNumber,
        [EmailAddress][MaxLength(200)] string? Email,
        [MaxLength(500)] string? ImageUrl,
        [MaxLength(100)] string? Category,

        // Owner credentials - admin creates these in the same call
        [Required][MaxLength(150)] string OwnerName,
        [Required][EmailAddress] string OwnerEmail,
        [Required][MinLength(8)] string OwnerPassword,
        [MaxLength(20)] string? OwnerPhone);

    public record UpdateRestaurantDto(
        [MaxLength(200)] string? Name,
        [MaxLength(500)] string? Description,
        [MaxLength(500)] string? Location,
        [MaxLength(20)] string? ContactNumber,
        [EmailAddress][MaxLength(200)] string? Email,
        [MaxLength(500)] string? ImageUrl,
        [MaxLength(100)] string? Category);
}
