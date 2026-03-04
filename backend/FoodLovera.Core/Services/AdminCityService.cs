using System;
using System.Threading;
using System.Threading.Tasks;
using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Services;

public sealed class AdminCityService(
    ICityRepository cities,
    IRestaurantRepository restaurants,
    IUnitOfWork uow) : IAdminCityService
{
    public async Task<int> CreateAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        var trimmed = name.Trim();
        var normalized = trimmed.ToLowerInvariant();

        if (await cities.ExistsByNameAsync(normalized, ct))
            throw new InvalidOperationException("City already exists.");

        var city = new City { Name = trimmed };

        await cities.AddAsync(city, ct);
        await uow.SaveChangesAsync(ct);

        return city.Id;
    }

    public async Task DeleteAsync(int cityId, CancellationToken ct)
    {
        if (cityId <= 0)
            throw new ArgumentException("Invalid cityId.", nameof(cityId));

        var city = await cities.GetByIdAsync(cityId, ct);
        if (city is null)
            throw new InvalidOperationException("City not found.");

        if (await restaurants.ExistsInCityAsync(cityId, ct))
            throw new InvalidOperationException("Cannot delete city because it has restaurants.");

        cities.Remove(city);
        await uow.SaveChangesAsync(ct);
    }
}