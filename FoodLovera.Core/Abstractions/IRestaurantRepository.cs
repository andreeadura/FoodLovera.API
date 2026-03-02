namespace FoodLovera.Core.Abstractions;

public interface IRestaurantRepository
{
    Task<Guid?> GetNextRestaurantIdAsync(HashSet<Guid> excludedRestaurantIds, CancellationToken ct);
    Task<Dictionary<Guid, string>> GetNamesByIdsAsync(IEnumerable<Guid> restaurantIds, CancellationToken ct);
}