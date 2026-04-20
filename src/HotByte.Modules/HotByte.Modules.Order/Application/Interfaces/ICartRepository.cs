using HotByte.Modules.Order.Domain.Entities;

namespace HotByte.Modules.Order.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(int userId);
        Task<Cart> CreateCartAsync(int userId);
        Task AddCartItemAsync(CartItem item);
        Task<CartItem?> GetCartItemAsync(int cartId, int menuItemId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task UpdateCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
    }
}
