using HotByte.Modules.Restaurant.Application.Interfaces;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Modules.Restaurant.Infrastructure
{
    public class RestaurantPublicService : IRestaurantPublicService
    {
        private readonly IRestaurantRepository _repository;

        public RestaurantPublicService(IRestaurantRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> RestaurantExistsAsync(int restaurantId)
        {
            var restaurant = await _repository.GetByIdAsync(restaurantId);
            return restaurant != null;
        }

        public async Task<string> GetRestaurantNameAsync(int restaurantId)
        {
            var restaurant = await _repository.GetByIdAsync(restaurantId);
            return restaurant?.Name ?? string.Empty;
        }

        public async Task<bool> IsOwnerOfRestaurantAsync(int userId, int restaurantId)
        {
            var restaurant = await _repository.GetByIdAsync(restaurantId);
            return restaurant != null && restaurant.OwnerUserId == userId;
        }

        public async Task<List<int>> GetOwnedRestaurantIdsAsync(int userId)
        {
            var restaurants = await _repository.GetByOwnerIdAsync(userId);
            return restaurants.Select(r => r.Id).ToList();
        }
    }
}
