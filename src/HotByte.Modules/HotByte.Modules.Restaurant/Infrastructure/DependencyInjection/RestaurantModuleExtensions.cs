using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HotByte.Modules.Restaurant.Application.Interfaces;
using HotByte.Modules.Restaurant.Application.Services;
using HotByte.Modules.Restaurant.Infrastructure.Repositories;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Modules.Restaurant.Infrastructure.DependencyInjection
{
    public static class RestaurantModuleExtensions
    {
        public static IServiceCollection AddRestaurantModule(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<RestaurantDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IRestaurantPublicService, RestaurantPublicService>();

            return services;
        }
    }
}
