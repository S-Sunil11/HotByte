using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Application.Mappings;
using HotByte.Modules.Order.Domain.Entities;
using HotByte.SharedKernel.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Order.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly INotificationService _notificationService;
        private readonly IRestaurantPublicService _restaurantPublicService;

        public OrderService(
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            INotificationService notificationService,
            IRestaurantPublicService restaurantPublicService)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _notificationService = notificationService;
            _restaurantPublicService = restaurantPublicService;
        }

        public async Task<OrderDto> PlaceOrderAsync(int userId, CreateOrderDto dto)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty. Cannot place order.");

            var restaurantId = cart.CartItems.First().RestaurantId;
            var restaurantName = await _restaurantPublicService.GetRestaurantNameAsync(restaurantId);
            var totalAmount = cart.CartItems.Sum(i => i.TotalPrice);

            var order = new Domain.Entities.Order
            {
                UserId = userId,
                RestaurantId = restaurantId,
                RestaurantName = restaurantName,
                Status = OrderStatus.Placed,
                TotalAmount = totalAmount,
                FinalAmount = totalAmount,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                ShippingAddress = dto.ShippingAddress,
                ContactNumber = dto.ContactNumber,
                EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(45),
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    MenuItemId = ci.MenuItemId,
                    MenuItemName = ci.MenuItemName,
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    TotalPrice = ci.TotalPrice,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            await _orderRepo.AddAsync(order);
            await _cartRepo.ClearCartAsync(cart.Id);

            // Requirement 1: email notification sent on order placement
            await _notificationService.SendNotificationAsync(
                userId, order.Id, "OrderPlaced",
                $"Your order #{order.Id} at {restaurantName} has been placed successfully! Total: {totalAmount:C}");

            return OrderMapper.ToDto(order);
        }

        // Requirement 6: user can see own order; admin can see any; owner can see only their restaurant's orders
        public async Task<OrderDto?> GetOrderByIdAsync(int id, int requesterUserId, string role)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return null;

            if (role == "Admin") return OrderMapper.ToDto(order);
            if (role == "Customer" && order.UserId == requesterUserId) return OrderMapper.ToDto(order);
            if (role == "RestaurantOwner")
            {
                var isOwner = await _restaurantPublicService.IsOwnerOfRestaurantAsync(requesterUserId, order.RestaurantId);
                if (isOwner) return OrderMapper.ToDto(order);
            }

            throw new UnauthorizedAccessException("You do not have access to this order.");
        }

        public async Task<List<OrderDto>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _orderRepo.GetByUserIdAsync(userId);
            return orders.Select(OrderMapper.ToDto).ToList();
        }

        // Requirement 7: owner asks for orders of a specific restaurant they own
        public async Task<List<OrderDto>> GetOrdersByRestaurantAsync(int restaurantId, int requesterUserId, string role)
        {
            if (role != "Admin")
            {
                var isOwner = await _restaurantPublicService.IsOwnerOfRestaurantAsync(requesterUserId, restaurantId);
                if (!isOwner)
                    throw new UnauthorizedAccessException("You do not own this restaurant.");
            }

            var orders = await _orderRepo.GetByRestaurantIdAsync(restaurantId);
            return orders.Select(OrderMapper.ToDto).ToList();
        }

        // Requirement 7: owner gets orders for ALL their restaurants (auto-detected from token)
        public async Task<List<OrderDto>> GetMyRestaurantsOrdersAsync(int ownerUserId)
        {
            var ownedRestaurantIds = await _restaurantPublicService.GetOwnedRestaurantIdsAsync(ownerUserId);
            if (!ownedRestaurantIds.Any()) return new List<OrderDto>();

            var orders = await _orderRepo.GetByRestaurantIdsAsync(ownedRestaurantIds);
            return orders.Select(OrderMapper.ToDto).ToList();
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllAsync();
            return orders.Select(OrderMapper.ToDto).ToList();
        }

        // Requirement 1: email sent on every status update. Requirement 6: ownership check.
        public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto, int requesterUserId, string role)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return false;

            if (role != "Admin")
            {
                var isOwner = await _restaurantPublicService.IsOwnerOfRestaurantAsync(requesterUserId, order.RestaurantId);
                if (!isOwner)
                    throw new UnauthorizedAccessException("You do not own the restaurant for this order.");
            }

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            if (dto.Status == OrderStatus.Delivered)
            {
                order.DeliveredAt = DateTime.UtcNow;
                order.PaymentStatus = PaymentStatus.Completed;
            }

            await _orderRepo.UpdateAsync(order);

            // Requirement 1: email on every status change
            var notifType = dto.Status switch
            {
                OrderStatus.Confirmed => "OrderConfirmed",
                OrderStatus.OutForDelivery => "OrderDispatched",
                OrderStatus.Delivered => "OrderDelivered",
                OrderStatus.Cancelled => "OrderCancelled",
                _ => "General"
            };

            await _notificationService.SendNotificationAsync(
                order.UserId, orderId, notifType,
                $"Your order #{orderId} status has been updated to: {dto.Status}");

            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null || order.UserId != userId) return false;
            if (order.Status != OrderStatus.Placed && order.Status != OrderStatus.Confirmed)
                return false;

            order.Status = OrderStatus.Cancelled;
            order.PaymentStatus = PaymentStatus.Refunded;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);

            await _notificationService.SendNotificationAsync(
                userId, orderId, "OrderCancelled",
                $"Your order #{orderId} has been cancelled.");

            return true;
        }
    }
}
