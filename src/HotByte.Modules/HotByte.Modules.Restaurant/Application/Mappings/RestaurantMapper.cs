using HotByte.Modules.Restaurant.Application.DTOs;

namespace HotByte.Modules.Restaurant.Application.Mappings
{
    public static class RestaurantMapper
    {
        public static RestaurantDto ToDto(Domain.Entities.Restaurant e)
            => new(
                e.Id,
                e.Name,
                e.Description,
                e.Location,
                e.ContactNumber,
                e.Email,
                e.ImageUrl,
                e.Status,
                e.OwnerUserId,
                e.Rating,
                e.TotalRatings,
                e.Category,
                e.CreatedAt);

        public static List<RestaurantDto> ToDtoList(List<Domain.Entities.Restaurant> entities)
            => entities.Select(ToDto).ToList();
    }
}
