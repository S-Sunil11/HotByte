using HotByte.Modules.Restaurant.Application.DTOs;

namespace HotByte.Modules.Restaurant.Application.Interfaces
{
    public interface IRestaurantService
    {
        // Requirement 8: creates restaurant AND owner account atomically
        Task<RestaurantDto> CreateRestaurantWithOwnerAsync(CreateRestaurantWithOwnerDto dto);
        Task<List<RestaurantDto>> GetAllRestaurantsAsync();
        Task<RestaurantDto?> GetRestaurantByIdAsync(int id);
        Task<List<RestaurantDto>> GetRestaurantsByOwnerAsync(int ownerUserId);

        // Requirement 2: search endpoint
        Task<List<RestaurantDto>> SearchRestaurantsAsync(string? name, string? category);

        // Requirement 6: ownership enforced inside service
        Task<bool> UpdateRestaurantAsync(int id, UpdateRestaurantDto dto, int userId, string role);
        Task<bool> DeleteRestaurantAsync(int id);
    }
}
