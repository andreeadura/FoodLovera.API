using FoodLovera.Core.Contracts;
using FoodLovera.Core.Helpers;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace FoodLovera.Infrastructure.Repositories;

public sealed class CityRepository : ICityRepository
{
    private readonly FoodLoveraDbContext _db;

    public CityRepository(FoodLoveraDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsAsync(int cityId, CancellationToken ct)
        => _db.Cities
            .AsNoTracking()
            .AnyAsync(c => c.Id == cityId, ct);

    public Task<int?> GetIdByNameAsync(string name, CancellationToken ct)
    {
        var normalized = name.Trim().ToLower();

        return _db.Cities.AsNoTracking()
            .Where(c => c.Name.ToLower() == normalized)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(ct);
    }
    public async Task<int?> GetIdByCityKeyAsync(string cityKey, CancellationToken ct)
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
    public Task<City?> GetByIdAsync(int cityId, CancellationToken ct)
    => _db.Cities.FirstOrDefaultAsync(c => c.Id == cityId, ct);

    public Task AddAsync(City city, CancellationToken ct)
        => _db.Cities.AddAsync(city, ct).AsTask();

    public void Remove(City city)
        => _db.Cities.Remove(city);

    public Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken ct)
        => _db.Cities.AsNoTracking()
            .AnyAsync(c => c.Name.ToLower() == normalizedName, ct);

    public async Task<IReadOnlyList<City>> GetAllAsync(CancellationToken ct)
    => await _db.Cities
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .ToListAsync(ct);
}