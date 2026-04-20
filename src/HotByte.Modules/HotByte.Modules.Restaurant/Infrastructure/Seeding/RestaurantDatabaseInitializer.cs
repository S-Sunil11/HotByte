namespace HotByte.Modules.Restaurant.Infrastructure.Seeding
{
    public static class RestaurantDatabaseInitializer
    {
        public static void SeedAll(RestaurantDbContext context)
        {
            RestaurantSeeder.Seed(context);
        }
    }
}
