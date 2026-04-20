using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Application.Services;
using HotByte.Modules.Order.Infrastructure.Email;
using HotByte.Modules.Order.Infrastructure.Repositories;

namespace HotByte.Modules.Order.Infrastructure.DependencyInjection
{
    public static class OrderModuleExtensions
    {
        public static IServiceCollection AddOrderModule(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationService, NotificationService>();

            // Real SMTP email service (Requirement 1)
            services.AddScoped<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}
