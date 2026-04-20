namespace HotByte.SharedKernel.Interfaces
{
    public interface IMenuPublicService
    {
        Task<bool> MenuItemExistsAsync(int menuItemId);
        Task<decimal> GetMenuItemPriceAsync(int menuItemId);
        Task<string> GetMenuItemNameAsync(int menuItemId);
        Task<int> GetMenuItemRestaurantIdAsync(int menuItemId);
    }
}
