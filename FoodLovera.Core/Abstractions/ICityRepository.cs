using System;
using System.Threading;
using System.Threading.Tasks;

namespace FoodLovera.Core.Abstractions;

public interface ICityRepository
{
    Task<bool> ExistsAsync(Guid cityId, CancellationToken ct);
    Task<Guid?> GetIdByNameAsync(string name, CancellationToken ct);
    Task<Guid?> GetIdByCityKeyAsync(string cityKey, CancellationToken ct);
}