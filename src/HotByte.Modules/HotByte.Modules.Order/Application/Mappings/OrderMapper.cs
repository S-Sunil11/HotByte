using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Domain.Entities;

namespace HotByte.Modules.Order.Application.Mappings
{
    public static class OrderMapper
    {
        public static CartItemDto ToDto(CartItem item)
            => new(
                item.Id,
                item.MenuItemId,
                item.MenuItemName,
                item.UnitPrice,
                item.Quantity,
                item.TotalPrice,
                item.RestaurantId);

        public static CartDto ToDto(Cart cart)
        {
            var items = cart.CartItems?.Select(ToDto).ToList() ?? new List<CartItemDto>();
            return new CartDto(cart.Id, cart.UserId, items, items.Sum(i => i.TotalPrice));
        }

        public static OrderItemDto ToDto(OrderItem item)
            => new(
                item.Id,
                item.MenuItemId,
                item.MenuItemName,
                item.UnitPrice,
                item.Quantity,
                item.TotalPrice);

        public static OrderDto ToDto(Domain.Entities.Order order)
            => new(
                order.Id,
                order.UserId,
                order.RestaurantId,
                order.RestaurantName,
                order.Status,
                order.TotalAmount,
                order.DiscountAmount,
                order.FinalAmount,
                order.PaymentMethod,
                order.PaymentStatus,
                order.ShippingAddress,
                order.ContactNumber,
                order.EstimatedDeliveryTime,
                order.DeliveredAt,
                order.Notes,
                order.CreatedAt,
                order.OrderItems?.Select(ToDto).ToList() ?? new List<OrderItemDto>());

        public static NotificationDto ToDto(OrderNotification n)
            => new(n.Id, n.UserId, n.OrderId, n.Type, n.Message, n.IsRead, n.SentAt);
    }
}
