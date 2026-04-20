namespace HotByte.Modules.Identity.Infrastructure.Seeding
{
    public static class IdentityDatabaseInitializer
    {
        public static void SeedAll(IdentityDbContext context)
        {
            IdentitySeeder.Seed(context);
        }
    }
}
