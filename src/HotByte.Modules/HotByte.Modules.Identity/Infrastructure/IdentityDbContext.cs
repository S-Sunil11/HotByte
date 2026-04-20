using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Identity.Domain.Entities;

namespace HotByte.Modules.Identity.Infrastructure
{
    public class IdentityDbContext
        : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options) { }

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Id)
                .HasColumnName("AuditID");

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.AppUser)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Status)
                .HasConversion<string>();

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
