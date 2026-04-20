using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HotByte.Modules.Menu.Application.Interfaces;
using HotByte.Modules.Menu.Application.Services;
using HotByte.Modules.Menu.Infrastructure.Repositories;
using HotByte.SharedKernel.Interfaces;

namespace HotByte.Modules.Menu.Infrastructure.DependencyInjection
{
    public static class MenuModuleExtensions
    {
        public static IServiceCollection AddMenuModule(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<MenuDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
            services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            services.AddScoped<IMenuItemRepository, MenuItemRepository>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<IMenuPublicService, MenuPublicService>();

            return services;
        }
    }
}
