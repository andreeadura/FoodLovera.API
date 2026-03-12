using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FoodLovera.Models.Models;

namespace FoodLovera.Core.Services;

public interface IAdminCityService
{
    Task<int> CreateAsync(string name, CancellationToken ct);
    Task DeleteAsync(int cityId, CancellationToken ct);

    Task<IReadOnlyList<AdminCityListItemDTO>> GetAllAsync(CancellationToken ct);
}