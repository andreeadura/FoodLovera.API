using FoodLovera.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class RestaurantRepository : IRestaurantRepository
{
    private readonly FoodLoveraDbContext _db;

    public RestaurantRepository(FoodLoveraDbContext db)
    {
        _db = db;
    }

    public async Task<Guid?> GetNextRestaurantIdAsync(
    Guid selectedCityId,
    bool useAllCategories,
    IReadOnlyCollection<Guid> selectedCategoryIds,
    IReadOnlyCollection<Guid> excludedRestaurantIds,
    CancellationToken ct)
    {
        var q = _db.Restaurants.AsNoTracking()
            .Where(r => r.IsActive)
            .Where(r => r.CityId == selectedCityId);

        if (!useAllCategories)
        {
            if (selectedCategoryIds.Count == 0) return null;

            q = q.Where(r => r.RestaurantCategories
                .Any(rc => selectedCategoryIds.Contains(rc.CategoryId)));
        }

        if (excludedRestaurantIds.Count > 0)
            q = q.Where(r => !excludedRestaurantIds.Contains(r.Id));

        return await q.OrderBy(r => r.Name)
            .Select(r => (Guid?)r.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Dictionary<Guid, string>> GetNamesByIdsAsync(IEnumerable<Guid> restaurantIds, CancellationToken ct)
    {
        var ids = restaurantIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<Guid, string>();

        var rows = await _db.Restaurants
            .AsNoTracking()
            .Where(r => ids.Contains(r.Id))
            .Select(r => new { r.Id, r.Name })
            .ToListAsync(ct);

        return rows.ToDictionary(x => x.Id, x => x.Name);
    }
}