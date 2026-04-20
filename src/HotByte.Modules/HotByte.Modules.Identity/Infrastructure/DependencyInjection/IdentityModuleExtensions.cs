using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotByte.Modules.Identity.Application.Interfaces;
using HotByte.Modules.Identity.Application.Services;
using HotByte.Modules.Identity.Domain.Entities;
using HotByte.Modules.Identity.Infrastructure.Repositories;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Modules.Identity.Infrastructure.DependencyInjection
{
    public static class IdentityModuleExtensions
    {
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            var jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is missing in configuration.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("RestaurantOwnerOnly", policy =>
                    policy.RequireRole("RestaurantOwner"));

                options.AddPolicy("CustomerOnly", policy =>
                    policy.RequireRole("Customer"));

                options.AddPolicy("AdminOrRestaurantOwner", policy =>
                    policy.RequireRole("Admin", "RestaurantOwner"));

                options.AddPolicy("AllAuthenticated", policy =>
                    policy.RequireAuthenticatedUser());
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            services.AddScoped<IAuthService, AuthService>();
            // UserService implements both IUserService and IUserPublicService
            services.AddScoped<UserService>();
            services.AddScoped<IUserService>(sp => sp.GetRequiredService<UserService>());
            services.AddScoped<IUserPublicService>(sp => sp.GetRequiredService<UserService>());

            return services;
        }
    }
}
