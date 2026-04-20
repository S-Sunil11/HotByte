using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Modules.Menu.Infrastructure
{
    public class MenuPublicService : IMenuPublicService
    {
        private readonly IMenuItemRepository _repository;

        public MenuPublicService(IMenuItemRepository repository) { _repository = repository; }

        public async Task<bool> MenuItemExistsAsync(int menuItemId)
        {
            var item = await _repository.GetByIdAsync(menuItemId);
            return item != null;
        }

        public async Task<decimal> GetMenuItemPriceAsync(int menuItemId)
        {
            var item = await _repository.GetByIdAsync(menuItemId);
            if (item == null) return 0;
            return item.DiscountPrice ?? item.Price;
        }

        public async Task<string> GetMenuItemNameAsync(int menuItemId)
        {
            var item = await _repository.GetByIdAsync(menuItemId);
            return item?.Name ?? string.Empty;
        }

        // Requirement 5: Cart derives RestaurantId from the menu item (server-side)
        public async Task<int> GetMenuItemRestaurantIdAsync(int menuItemId)
        {
            var item = await _repository.GetByIdAsync(menuItemId);
            return item?.RestaurantId ?? 0;
        }
    }
}
