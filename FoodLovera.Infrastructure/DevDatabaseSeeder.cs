using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure;

public static class DevDatabaseSeeder
{
    public static async Task SeedAsync(FoodLoveraDbContext db, CancellationToken ct)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Restaurants.AnyAsync(ct))
            return;

        var now = DateTime.UtcNow;

        var cluj = new City { Name = "Cluj-Napoca" };
        var buc = new City { Name = "Bucuresti" };
        var timisoara = new City { Name = "Timisoara" };
        var iasi = new City { Name = "Iasi" };
        var constanta = new City { Name = "Constanta" };

        await db.Cities.AddRangeAsync([cluj, buc, timisoara, iasi, constanta], ct);
        await db.SaveChangesAsync(ct); 

        var italian = new Category { Name = "Italian" };
        var sushi = new Category { Name = "Sushi" };
        var burger = new Category { Name = "Burger" };
        var vegan = new Category { Name = "Vegan" };
        var romanian = new Category { Name = "Romanian" };

        await db.Categories.AddRangeAsync([italian, sushi, burger, vegan, romanian], ct);
        await db.SaveChangesAsync(ct); 

        var r1 = new Restaurant
        {
            Name = "Pasta House",
            CityId = cluj.Id,
            ImageUrl = "https://picsum.photos/seed/pasta/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r2 = new Restaurant
        {
            Name = "Sushi Corner",
            CityId = cluj.Id,
            ImageUrl = "https://picsum.photos/seed/sushi/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r3 = new Restaurant
        {
            Name = "Burger Lab",
            CityId = buc.Id,
            ImageUrl = "https://picsum.photos/seed/burger/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r4 = new Restaurant
        {
            Name = "Green Bowl",
            CityId = buc.Id,
            ImageUrl = "https://picsum.photos/seed/vegan/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r5 = new Restaurant
        {
            Name = "Casa Bunicii",
            CityId = cluj.Id,
            ImageUrl = "https://picsum.photos/seed/romanian/600/400",
            IsActive = true,
            CreatedAt = now
        };

        await db.Restaurants.AddRangeAsync([r1, r2, r3, r4, r5], ct);
        await db.SaveChangesAsync(ct); 

        await db.RestaurantCategories.AddRangeAsync(
            [
                new RestaurantCategory { RestaurantId = r1.Id, CategoryId = italian.Id },
                new RestaurantCategory { RestaurantId = r2.Id, CategoryId = sushi.Id },
                new RestaurantCategory { RestaurantId = r3.Id, CategoryId = burger.Id },
                new RestaurantCategory { RestaurantId = r4.Id, CategoryId = vegan.Id },
                new RestaurantCategory { RestaurantId = r5.Id, CategoryId = romanian.Id },
            ],
            ct
        );

        await db.SaveChangesAsync(ct);
    }
}