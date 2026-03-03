using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
}