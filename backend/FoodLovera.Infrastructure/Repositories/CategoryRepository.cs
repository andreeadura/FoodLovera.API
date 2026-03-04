using FoodLovera.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly FoodLoveraDbContext _db;
    public CategoryRepository(FoodLoveraDbContext db) => _db = db;

    public async Task<HashSet<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        var list = ids.Distinct().ToList();
        if (list.Count == 0) return new HashSet<int>();

        var existing = await _db.Categories.AsNoTracking()
            .Where(c => list.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(ct);

        return existing.ToHashSet();
    }
}