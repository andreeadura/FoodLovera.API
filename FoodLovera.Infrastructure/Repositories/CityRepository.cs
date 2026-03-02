using FoodLovera.Core.Abstractions;
using FoodLovera.Core.Helpers;
using Microsoft.EntityFrameworkCore;


namespace FoodLovera.Infrastructure.Repositories;

public sealed class CityRepository : ICityRepository
{
    private readonly FoodLoveraDbContext _db;

    public CityRepository(FoodLoveraDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsAsync(Guid cityId, CancellationToken ct)
        => _db.Cities
            .AsNoTracking()
            .AnyAsync(c => c.Id == cityId, ct);

    public Task<Guid?> GetIdByNameAsync(string name, CancellationToken ct)
    {
        var normalized = name.Trim().ToLower();

        return _db.Cities.AsNoTracking()
            .Where(c => c.Name.ToLower() == normalized)
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(ct);
    }
    public async Task<Guid?> GetIdByCityKeyAsync(string cityKey, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cityKey))
            return null;

      
        var cities = await _db.Cities
            .AsNoTracking()
            .Select(c => new { c.Id, c.Name })
            .ToListAsync(ct);

        foreach (var c in cities)
        {
            var key = CityNameNormalizer.ToCityKey(c.Name); 
            if (key == cityKey)
                return c.Id;
        }

        return null;
    }
}