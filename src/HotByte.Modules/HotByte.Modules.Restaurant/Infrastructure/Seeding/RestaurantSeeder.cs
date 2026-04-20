namespace HotByte.Modules.Restaurant.Infrastructure.Seeding
{
    public static class RestaurantSeeder
    {
        public static void Seed(RestaurantDbContext context)
        {
            // No restaurants seeded by default.
            // Admin creates restaurants + owner accounts via POST /api/restaurants (requirement 8).
            _ = context;
        }
    }
}
