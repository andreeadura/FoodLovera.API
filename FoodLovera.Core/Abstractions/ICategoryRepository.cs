using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FoodLovera.Core.Abstractions;

public interface ICategoryRepository
{
    Task<HashSet<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
}