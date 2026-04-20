using Microsoft.AspNetCore.Identity;
using HotByte.Modules.Identity.Domain.Entities;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Identity.Infrastructure.Seeding
{
    public static class IdentitySeeder
    {
        public static void Seed(IdentityDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new[] { "Customer", "RestaurantOwner", "Admin" };

                foreach (var roleName in roles)
                {
                    context.Roles.Add(new IdentityRole<int>
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<AppUser>();
                var users = new List<AppUser>();

                // Seed 2 admins + 8 customers only. RestaurantOwners are created by admin via POST /api/restaurants
                var roleGroups = new List<(UserRole Role, int Count, string NamePrefix, string Pwd)>
                {
                    (UserRole.Admin,    2, "Admin User", "Admin@123!"),
                    (UserRole.Customer, 8, "Customer",   "Cust@1234!")
                };

                int phoneCounter = 1;

                foreach (var group in roleGroups)
                {
                    for (int i = 1; i <= group.Count; i++)
                    {
                        var email = $"{group.Role.ToString().ToLower()}{i}@hotbyte.com";
                        var user = new AppUser
                        {
                            UserName = email,
                            NormalizedUserName = email.ToUpper(),
                            Email = email,
                            NormalizedEmail = email.ToUpper(),
                            Name = $"{group.NamePrefix} {i}",
                            Role = group.Role,
                            Status = UserStatus.Active,
                            Gender = i % 2 == 0 ? "Male" : "Female",
                            Address = $"{100 + i} Main Street, City",
                            PhoneNumber = $"555-{phoneCounter:D4}",
                            EmailConfirmed = true,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            CreatedAt = DateTime.UtcNow
                        };

                        user.PasswordHash = hasher.HashPassword(user, group.Pwd);
                        users.Add(user);
                        phoneCounter++;
                    }
                }

                context.Users.AddRange(users);
                context.SaveChanges();

                foreach (var user in users)
                {
                    var roleName = user.Role.ToString();
                    var roleId = context.Roles.First(r => r.Name == roleName).Id;
                    context.UserRoles.Add(new IdentityUserRole<int>
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    });
                }
                context.SaveChanges();
            }
        }
    }
}
