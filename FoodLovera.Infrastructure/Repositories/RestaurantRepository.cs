using FoodLovera.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class RestaurantRepository : IRestaurantRepository
{
    private readonly FoodLoveraDbContext _db;
    public RestaurantRepository(FoodLoveraDbContext db) => _db = db;

    public async Task<Guid?> GetNextRestaurantIdAsync(HashSet<Guid> excludedRestaurantIds, CancellationToken ct)
    {
        var query = _db.Restaurants.AsNoTracking().Where(r => r.IsActive);

        if (excludedRestaurantIds.Count > 0)
            query = query.Where(r => !excludedRestaurantIds.Contains(r.Id));

        return await query
            .OrderBy(r => r.Name)
            .Select(r => (Guid?)r.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Dictionary<Guid, string>> GetNamesByIdsAsync(IEnumerable<Guid> restaurantIds, CancellationToken ct)
    {
        var ids = restaurantIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<Guid, string>();

        var rows = await _db.Restaurants.AsNoTracking()
            .Where(r => ids.Contains(r.Id))
            .Select(r => new { r.Id, r.Name })
            .ToListAsync(ct);

        return rows.ToDictionary(x => x.Id, x => x.Name);
    }
}