using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Menu.Domain.Entities;

namespace HotByte.Modules.Menu.Infrastructure
{
    public class MenuDbContext : DbContext
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options) { }

        public DbSet<MenuCategory> MenuCategories { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuCategory>()
                .Property(c => c.Id).HasColumnName("CategoryID");

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Id).HasColumnName("MenuItemID");

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.AvailabilityTime).HasConversion<string>();
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.DietaryType).HasConversion<string>();
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.TasteInfo).HasConversion<string>();

            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>().HasIndex(m => m.RestaurantId);
            modelBuilder.Entity<MenuItem>().HasIndex(m => m.CategoryId);
            modelBuilder.Entity<MenuItem>().HasIndex(m => m.Name);
        }
    }
}
