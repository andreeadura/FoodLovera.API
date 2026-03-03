using System.Threading;
using System.Threading.Tasks;

namespace FoodLovera.Core.Services;

public interface IAdminCityService
{
    Task<int> CreateAsync(string name, CancellationToken ct);
    Task DeleteAsync(int cityId, CancellationToken ct);
}