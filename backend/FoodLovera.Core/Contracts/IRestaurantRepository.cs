using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface IRestaurantRepository
{
    Task<int?> GetNextRestaurantIdAsync(
        int selectedCityId,
        bool useAllCategories,
        IReadOnlyCollection<int> selectedCategoryIds,
        IReadOnlyCollection<int> excludedRestaurantIds,
        CancellationToken ct);

    Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> restaurantIds, CancellationToken ct);
    Task<Restaurant?> GetByIdAsync(int id, CancellationToken ct);
    Task<Restaurant?> GetByIdWithCategoriesAsync(int id, CancellationToken ct);
    Task AddAsync(Restaurant restaurant, CancellationToken ct);
    void Remove(Restaurant restaurant);
    Task<bool> ExistsInCityAsync(int cityId, CancellationToken ct);
    Task<IReadOnlyList<Restaurant>> GetAllWithCityAsync(CancellationToken ct);
}