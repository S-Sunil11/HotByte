
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using HotByte.Modules.Identity.Infrastructure.DependencyInjection;
using HotByte.Modules.Order.Infrastructure.DependencyInjection;
using HotByte.Modules.Restaurant.Infrastructure.DependencyInjection;
using HotByte.Modules.Menu.Infrastructure.DependencyInjection;

namespace HotByte.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(HotByte.Modules.Identity.Controllers.AuthController).Assembly)
                .AddApplicationPart(typeof(HotByte.Modules.Restaurant.Controllers.RestaurantController).Assembly)
                .AddApplicationPart(typeof(HotByte.Modules.Menu.Controllers.MenuItemController).Assembly)
                .AddApplicationPart(typeof(HotByte.Modules.Order.Controllers.OrderController).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters
                        .Add(new JsonStringEnumConverter());
                });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HotByte API",
                    Version = "v1",
                    Description = "API for HotByte Food Ordering Application"
                });

                options.UseInlineDefinitionsForEnums();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Register modules
            services.AddIdentityModule(config);
            services.AddRestaurantModule(config);
            services.AddMenuModule(config);
            services.AddOrderModule(config);

            return services;
        }
    }
}
