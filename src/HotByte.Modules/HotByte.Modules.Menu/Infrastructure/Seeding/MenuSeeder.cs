using HotByte.Modules.Menu.Domain.Entities;

namespace HotByte.Modules.Menu.Infrastructure.Seeding
{
    public static class MenuSeeder
    {
        public static void Seed(MenuDbContext context)
        {
            // Seed default categories. Menu items are created by restaurant owners after admin sets them up.
            if (!context.MenuCategories.Any())
            {
                var categories = new List<MenuCategory>
                {
                    new() { Name = "Appetizer", Description = "Starters and small bites" },
                    new() { Name = "Main Course", Description = "Full meal dishes" },
                    new() { Name = "Dessert", Description = "Sweet treats and pastries" },
                    new() { Name = "Burger", Description = "Gourmet burgers" },
                    new() { Name = "Pizza", Description = "Italian pizzas" },
                    new() { Name = "Italian", Description = "Italian cuisine" },
                    new() { Name = "Arabian", Description = "Middle Eastern dishes" },
                    new() { Name = "Beverage", Description = "Drinks and refreshments" }
                };
                context.MenuCategories.AddRange(categories);
                context.SaveChanges();
            }
        }
    }
}
