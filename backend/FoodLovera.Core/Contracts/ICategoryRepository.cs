using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FoodLovera.Core.Contracts;

public interface ICategoryRepository
{
    Task<HashSet<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct);
}