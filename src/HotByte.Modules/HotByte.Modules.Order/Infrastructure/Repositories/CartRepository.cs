using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Domain.Entities;

namespace HotByte.Modules.Order.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly OrderDbContext _context;

        public CartRepository(OrderDbContext context) { _context = context; }

        public async Task<Cart?> GetByUserIdAsync(int userId)
            => await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

        public async Task<Cart> CreateCartAsync(int userId)
        {
            var cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task AddCartItemAsync(CartItem item)
        {
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int menuItemId)
            => await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.MenuItemId == menuItemId);

        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
            => await _context.CartItems.FindAsync(cartItemId);

        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            var items = await _context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
