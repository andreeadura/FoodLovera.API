using System.Threading;
using System.Threading.Tasks;

namespace FoodLovera.Core.Services;

public interface IAdminRestaurantService
{
    Task<int> CreateAsync(string name, int cityId, string imageUrl, bool isActive, CancellationToken ct);
    Task DeleteAsync(int restaurantId, CancellationToken ct);
}