using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FoodLovera.Core.Abstractions;

public interface IRestaurantRepository
{
    Task<Guid?> GetNextRestaurantIdAsync(
        Guid selectedCityId,
        bool useAllCategories,
        IReadOnlyCollection<Guid> selectedCategoryIds,
        IReadOnlyCollection<Guid> excludedRestaurantIds,
        CancellationToken ct);

    Task<Dictionary<Guid, string>> GetNamesByIdsAsync(IEnumerable<Guid> restaurantIds, CancellationToken ct);
}