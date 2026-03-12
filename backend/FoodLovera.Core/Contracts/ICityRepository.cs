using FoodLovera.Models.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FoodLovera.Core.Contracts;

public interface ICityRepository
{
    Task<bool> ExistsAsync(int cityId, CancellationToken ct);
    Task<int?> GetIdByNameAsync(string name, CancellationToken ct);
    Task<int?> GetIdByCityKeyAsync(string cityKey, CancellationToken ct);
    Task<City?> GetByIdAsync(int cityId, CancellationToken ct);
    Task AddAsync(City city, CancellationToken ct);
    Task<IReadOnlyList<City>> GetAllAsync(CancellationToken ct);
    void Remove(City city);
    Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken ct);
}