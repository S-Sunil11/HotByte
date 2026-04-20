namespace HotByte.Modules.Menu.Infrastructure.Seeding
{
    public static class MenuDatabaseInitializer
    {
        public static void SeedAll(MenuDbContext context)
        {
            MenuSeeder.Seed(context);
        }
    }
}
