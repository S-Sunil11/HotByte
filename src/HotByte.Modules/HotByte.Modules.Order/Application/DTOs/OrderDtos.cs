using System.ComponentModel.DataAnnotations;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Order.Application.DTOs
{
    public record OrderDto(
        int Id,
        int UserId,
        int RestaurantId,
        string? RestaurantName,
        OrderStatus Status,
        decimal TotalAmount,
        decimal? DiscountAmount,
        decimal FinalAmount,
        PaymentMethod PaymentMethod,
        PaymentStatus PaymentStatus,
        string? ShippingAddress,
        string? ContactNumber,
        DateTime? EstimatedDeliveryTime,
        DateTime? DeliveredAt,
        string? Notes,
        DateTime CreatedAt,
        List<OrderItemDto> OrderItems);

    public record OrderItemDto(
        int Id,
        int MenuItemId,
        string MenuItemName,
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice);

    public record CreateOrderDto(
        [MaxLength(500)] string? ShippingAddress,
        [MaxLength(20)] string? ContactNumber,
        PaymentMethod PaymentMethod = PaymentMethod.CashOnDelivery,
        [MaxLength(500)] string? Notes = null);

    public record UpdateOrderStatusDto([Required] OrderStatus Status);
}
