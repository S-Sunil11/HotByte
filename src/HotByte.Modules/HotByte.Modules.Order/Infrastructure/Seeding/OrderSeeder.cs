namespace HotByte.Modules.Order.Infrastructure.Seeding
{
    public static class OrderSeeder
    {
        public static void Seed(OrderDbContext context)
        {
            // No orders or notifications seeded by default.
            // Orders are created by customers via POST /api/orders.
            _ = context;
        }
    }
}
