using System;
using System.Threading;
using System.Threading.Tasks;

namespace FoodLovera.Core.Contracts;

public interface ICityRepository
{
    Task<bool> ExistsAsync(int cityId, CancellationToken ct);
    Task<int?> GetIdByNameAsync(string name, CancellationToken ct);
    Task<int?> GetIdByCityKeyAsync(string cityKey, CancellationToken ct);
}