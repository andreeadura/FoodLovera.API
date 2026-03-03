using System;
using System.Threading;
using System.Threading.Tasks;
using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Services;

public sealed class AdminRestaurantService(
    IRestaurantRepository restaurants,
    ICityRepository cities,
    IUnitOfWork uow) : IAdminRestaurantService
{
    public async Task<int> CreateAsync(string name, int cityId, string imageUrl, bool isActive, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (cityId <= 0)
            throw new ArgumentException("CityId is required.", nameof(cityId));
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("ImageUrl is required.", nameof(imageUrl));

        if (!await cities.ExistsAsync(cityId, ct))
            throw new InvalidOperationException("City not found.");

        var restaurant = new Restaurant
        {
            Name = name.Trim(),
            CityId = cityId,
            ImageUrl = imageUrl.Trim(),
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };

        await restaurants.AddAsync(restaurant, ct);
        await uow.SaveChangesAsync(ct);

        return restaurant.Id;
    }

    public async Task DeleteAsync(int restaurantId, CancellationToken ct)
    {
        if (restaurantId <= 0)
            throw new ArgumentException("Invalid restaurantId.", nameof(restaurantId));

        var restaurant = await restaurants.GetByIdAsync(restaurantId, ct);
        if (restaurant is null)
            throw new InvalidOperationException("Restaurant not found.");

        restaurants.Remove(restaurant);
        await uow.SaveChangesAsync(ct);
    }
}