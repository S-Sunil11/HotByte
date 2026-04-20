namespace HotByte.Modules.Order.Infrastructure.Seeding
{
    public static class OrderDatabaseInitializer
    {
        public static void SeedAll(OrderDbContext context)
        {
            OrderSeeder.Seed(context);
        }
    }
}
