using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface ICategoryRepository
{
    Task<HashSet<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct);
}