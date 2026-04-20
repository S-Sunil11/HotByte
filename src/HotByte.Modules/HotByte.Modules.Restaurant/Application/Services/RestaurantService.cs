using HotByte.Modules.Restaurant.Application.DTOs;
using HotByte.Modules.Restaurant.Application.Interfaces;
using HotByte.Modules.Restaurant.Application.Mappings;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Modules.Restaurant.Application.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _repository;
        private readonly IUserPublicService _userPublicService;

        public RestaurantService(
            IRestaurantRepository repository,
            IUserPublicService userPublicService)
        {
            _repository = repository;
            _userPublicService = userPublicService;
        }

        // Requirement 8: Creates owner account AND restaurant in one call.
        public async Task<RestaurantDto> CreateRestaurantWithOwnerAsync(CreateRestaurantWithOwnerDto dto)
        {
            var ownerUserId = await _userPublicService.CreateRestaurantOwnerAccountAsync(
                dto.OwnerName,
                dto.OwnerEmail,
                dto.OwnerPassword,
                dto.OwnerPhone);

            var entity = new Domain.Entities.Restaurant
            {
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.Location,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                ImageUrl = dto.ImageUrl,
                Category = dto.Category,
                OwnerUserId = ownerUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            return RestaurantMapper.ToDto(entity);
        }

        public async Task<List<RestaurantDto>> GetAllRestaurantsAsync()
        {
            var restaurants = await _repository.GetAllAsync();
            return RestaurantMapper.ToDtoList(restaurants);
        }

        public async Task<RestaurantDto?> GetRestaurantByIdAsync(int id)
        {
            var restaurant = await _repository.GetByIdAsync(id);
            return restaurant == null ? null : RestaurantMapper.ToDto(restaurant);
        }

        public async Task<List<RestaurantDto>> GetRestaurantsByOwnerAsync(int ownerUserId)
        {
            var restaurants = await _repository.GetByOwnerIdAsync(ownerUserId);
            return RestaurantMapper.ToDtoList(restaurants);
        }

        public async Task<List<RestaurantDto>> SearchRestaurantsAsync(string? name, string? category)
        {
            var results = await _repository.SearchAsync(name, category);
            return RestaurantMapper.ToDtoList(results);
        }

        // Requirement 6: owners can only update their own restaurants. Admins can update any.
        public async Task<bool> UpdateRestaurantAsync(int id, UpdateRestaurantDto dto, int userId, string role)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            if (role != "Admin" && entity.OwnerUserId != userId)
                throw new UnauthorizedAccessException("You do not own this restaurant.");

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.Location != null) entity.Location = dto.Location;
            if (dto.ContactNumber != null) entity.ContactNumber = dto.ContactNumber;
            if (dto.Email != null) entity.Email = dto.Email;
            if (dto.ImageUrl != null) entity.ImageUrl = dto.ImageUrl;
            if (dto.Category != null) entity.Category = dto.Category;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteRestaurantAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
