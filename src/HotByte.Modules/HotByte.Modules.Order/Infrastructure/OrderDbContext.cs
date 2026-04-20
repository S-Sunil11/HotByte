using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Order.Domain.Entities;

namespace HotByte.Modules.Order.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Domain.Entities.Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderNotification> OrderNotifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cart>().Property(c => c.Id).HasColumnName("CartID");
            modelBuilder.Entity<CartItem>().Property(c => c.Id).HasColumnName("CartItemID");
            modelBuilder.Entity<Domain.Entities.Order>().Property(o => o.Id).HasColumnName("OrderID");
            modelBuilder.Entity<OrderItem>().Property(o => o.Id).HasColumnName("OrderItemID");
            modelBuilder.Entity<OrderNotification>().Property(n => n.Id).HasColumnName("NotificationID");

            modelBuilder.Entity<Domain.Entities.Order>().Property(o => o.Status).HasConversion<string>();
            modelBuilder.Entity<Domain.Entities.Order>().Property(o => o.PaymentMethod).HasConversion<string>();
            modelBuilder.Entity<Domain.Entities.Order>().Property(o => o.PaymentStatus).HasConversion<string>();
            modelBuilder.Entity<OrderNotification>().Property(n => n.Type).HasConversion<string>();

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>().HasIndex(c => c.UserId).IsUnique();
            modelBuilder.Entity<Domain.Entities.Order>().HasIndex(o => o.UserId);
            modelBuilder.Entity<Domain.Entities.Order>().HasIndex(o => o.RestaurantId);
            modelBuilder.Entity<OrderNotification>().HasIndex(n => n.UserId);
        }
    }
}
