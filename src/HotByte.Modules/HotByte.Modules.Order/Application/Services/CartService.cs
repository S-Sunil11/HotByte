using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Application.Mappings;
using HotByte.Modules.Order.Domain.Entities;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Modules.Order.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IMenuPublicService _menuService;

        public CartService(ICartRepository cartRepo, IMenuPublicService menuService)
        {
            _cartRepo = cartRepo;
            _menuService = menuService;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId)
                ?? await _cartRepo.CreateCartAsync(userId);
            return OrderMapper.ToDto(cart);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            // Requirement 5: Derive RestaurantId from the menu item (server-side) — never trust client
            var menuItemExists = await _menuService.MenuItemExistsAsync(dto.MenuItemId);
            if (!menuItemExists)
                throw new InvalidOperationException($"Menu item {dto.MenuItemId} does not exist.");

            var restaurantId = await _menuService.GetMenuItemRestaurantIdAsync(dto.MenuItemId);
            if (restaurantId == 0)
                throw new InvalidOperationException($"Menu item {dto.MenuItemId} is not linked to a restaurant.");

            var cart = await _cartRepo.GetByUserIdAsync(userId)
                ?? await _cartRepo.CreateCartAsync(userId);

            // Requirement 3: Cart can only have items from ONE restaurant
            if (cart.CartItems != null && cart.CartItems.Any())
            {
                var existingRestaurantId = cart.CartItems.First().RestaurantId;
                if (existingRestaurantId != restaurantId)
                {
                    throw new InvalidOperationException(
                        "Your cart already has items from a different restaurant. " +
                        "Clear your cart before adding items from another restaurant.");
                }
            }

            var existingItem = await _cartRepo.GetCartItemAsync(cart.Id, dto.MenuItemId);
            var price = await _menuService.GetMenuItemPriceAsync(dto.MenuItemId);
            var name = await _menuService.GetMenuItemNameAsync(dto.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepo.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    MenuItemId = dto.MenuItemId,
                    MenuItemName = name,
                    UnitPrice = price,
                    Quantity = dto.Quantity,
                    TotalPrice = price * dto.Quantity,
                    RestaurantId = restaurantId,
                    CreatedAt = DateTime.UtcNow
                };
                await _cartRepo.AddCartItemAsync(cartItem);
            }

            var updatedCart = await _cartRepo.GetByUserIdAsync(userId);
            return OrderMapper.ToDto(updatedCart!);
        }

        public async Task<bool> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            var item = await _cartRepo.GetCartItemByIdAsync(cartItemId);
            if (item == null) return false;

            // Requirement 6: ownership - user can only modify their own cart items
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null || item.CartId != cart.Id)
                throw new UnauthorizedAccessException("This cart item does not belong to you.");

            item.Quantity = dto.Quantity;
            item.TotalPrice = item.UnitPrice * dto.Quantity;
            item.UpdatedAt = DateTime.UtcNow;
            await _cartRepo.UpdateCartItemAsync(item);
            return true;
        }

        public async Task<bool> RemoveCartItemAsync(int userId, int cartItemId)
        {
            var item = await _cartRepo.GetCartItemByIdAsync(cartItemId);
            if (item == null) return false;

            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null || item.CartId != cart.Id)
                throw new UnauthorizedAccessException("This cart item does not belong to you.");

            await _cartRepo.RemoveCartItemAsync(cartItemId);
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null) return false;
            await _cartRepo.ClearCartAsync(cart.Id);
            return true;
        }
    }
}
