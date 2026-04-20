using Microsoft.EntityFrameworkCore;
using HotByte.Api.Extensions;
using HotByte.Modules.Identity.Infrastructure;
using HotByte.Modules.Identity.Infrastructure.Seeding;
using HotByte.Modules.Menu.Infrastructure;
using HotByte.Modules.Menu.Infrastructure.Seeding;
using HotByte.Modules.Order.Infrastructure;
using HotByte.Modules.Order.Infrastructure.Seeding;
using HotByte.Modules.Restaurant.Infrastructure;
using HotByte.Modules.Restaurant.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var identityDb = services.GetRequiredService<IdentityDbContext>();
        var restaurantDb = services.GetRequiredService<RestaurantDbContext>();
        var menuDb = services.GetRequiredService<MenuDbContext>();
        var orderDb = services.GetRequiredService<OrderDbContext>();

        identityDb.Database.Migrate();
        restaurantDb.Database.Migrate();
        menuDb.Database.Migrate();
        orderDb.Database.Migrate();

        IdentityDatabaseInitializer.SeedAll(identityDb);
        RestaurantDatabaseInitializer.SeedAll(restaurantDb);
        MenuDatabaseInitializer.SeedAll(menuDb);
        OrderDatabaseInitializer.SeedAll(orderDb);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration or seeding.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HotByte API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
