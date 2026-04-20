using Microsoft.EntityFrameworkCore;

namespace HotByte.Modules.Restaurant.Infrastructure
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options) { }

        public DbSet<Domain.Entities.Restaurant> Restaurants { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domain.Entities.Restaurant>()
                .Property(r => r.Id)
                .HasColumnName("RestaurantID");

            modelBuilder.Entity<Domain.Entities.Restaurant>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Domain.Entities.Restaurant>()
                .HasIndex(r => r.OwnerUserId);

            modelBuilder.Entity<Domain.Entities.Restaurant>()
                .HasIndex(r => r.Name);

            modelBuilder.Entity<Domain.Entities.Restaurant>()
                .HasIndex(r => r.Category);
        }
    }
}
