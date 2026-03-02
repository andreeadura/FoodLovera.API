using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure;

public static class DevDatabaseSeeder
{
    public static async Task SeedAsync(FoodLoveraDbContext db, CancellationToken ct)
    {
       
        await db.Database.MigrateAsync(ct);

        //seed doar daca e gol
        if (await db.Restaurants.AnyAsync(ct))
            return;

        var cluj = new City { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Cluj-Napoca" };
        var buc = new City { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "București" };

        var italian = new Category { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Italian" };
        var sushi = new Category { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Sushi" };
        var burger = new Category { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Burger" };
        var vegan = new Category { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Name = "Vegan" };
        var romanian = new Category { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Name = "Romanian" };

        var now = DateTime.UtcNow;

        var r1 = new Restaurant
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Pasta House",
            CityId = cluj.Id,
            ImageUrl = "https://picsum.photos/seed/pasta/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r2 = new Restaurant
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Name = "Sushi Corner",
            CityId = cluj.Id,
            ImageUrl = "https://picsum.photos/seed/sushi/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r3 = new Restaurant
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            Name = "Burger Lab",
            CityId = buc.Id,
            ImageUrl = "https://picsum.photos/seed/burger/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r4 = new Restaurant
        {
            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            Name = "Green Bowl",
            CityId = buc.Id,
            ImageUrl = "https://picsum.photos/seed/vegan/600/400",
            IsActive = true,
            CreatedAt = now
        };

        var r5 = new Restaurant
        {
            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            Name = "Casa Bunicii",
            CityId = cluj.Id,
            ImageUrl = "https://picsum.photos/seed/romanian/600/400",
            IsActive = true,
            CreatedAt = now
        };

        await db.Cities.AddRangeAsync([cluj, buc], ct);
        await db.Categories.AddRangeAsync([italian, sushi, burger, vegan, romanian], ct);
        await db.Restaurants.AddRangeAsync([r1, r2, r3, r4, r5], ct);

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