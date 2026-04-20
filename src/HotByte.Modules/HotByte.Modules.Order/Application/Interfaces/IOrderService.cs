using HotByte.Modules.Order.Application.DTOs;

namespace HotByte.Modules.Order.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> PlaceOrderAsync(int userId, CreateOrderDto dto);

        // Requirement 6: ownership check on get by id
        Task<OrderDto?> GetOrderByIdAsync(int id, int requesterUserId, string role);
        Task<List<OrderDto>> GetOrdersByUserAsync(int userId);

        // Requirement 7: owner accesses orders for a specific restaurant (with ownership check)
        Task<List<OrderDto>> GetOrdersByRestaurantAsync(int restaurantId, int requesterUserId, string role);

        // Requirement 7: owner sees ALL orders across all restaurants they own (auto-detect from token)
        Task<List<OrderDto>> GetMyRestaurantsOrdersAsync(int ownerUserId);

        Task<List<OrderDto>> GetAllOrdersAsync();

        // Requirement 6: ownership check on status update
        Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto, int requesterUserId, string role);
        Task<bool> CancelOrderAsync(int orderId, int userId);
    }
}
