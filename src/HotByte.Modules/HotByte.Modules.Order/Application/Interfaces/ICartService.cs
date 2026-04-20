using HotByte.Modules.Order.Application.DTOs;

namespace HotByte.Modules.Order.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);
        Task<bool> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task<bool> RemoveCartItemAsync(int userId, int cartItemId);
        Task<bool> ClearCartAsync(int userId);
    }
}
