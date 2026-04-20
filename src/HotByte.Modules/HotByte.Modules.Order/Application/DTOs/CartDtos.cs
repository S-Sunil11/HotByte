using System.ComponentModel.DataAnnotations;

namespace HotByte.Modules.Order.Application.DTOs
{
    public record CartDto(
        int Id,
        int UserId,
        List<CartItemDto> CartItems,
        decimal TotalAmount);

    public record CartItemDto(
        int Id,
        int MenuItemId,
        string MenuItemName,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice,
        int RestaurantId);

    // Requirement 5: NO RestaurantId from client — derived server-side from the menu item
    public record AddToCartDto(
        [Required] int MenuItemId,
        [Range(1, 100)] int Quantity = 1);

    public record UpdateCartItemDto([Required][Range(1, 100)] int Quantity);
}
