namespace HotByte.SharedKernel.Interfaces
{
    public interface IRestaurantPublicService
    {
        Task<bool> RestaurantExistsAsync(int restaurantId);
        Task<string> GetRestaurantNameAsync(int restaurantId);
        Task<bool> IsOwnerOfRestaurantAsync(int userId, int restaurantId);
        Task<List<int>> GetOwnedRestaurantIdsAsync(int userId);
    }
}
