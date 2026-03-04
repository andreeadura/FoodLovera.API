using FoodLovera.Models.Models;

namespace FoodLovera.Core.Services;

public interface IUserService
{
    Task ChangeUsernameAsync(int userId, ChangeUsernameRequestDTO request, CancellationToken ct);
}