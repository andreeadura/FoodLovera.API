using FoodLovera.Models.Models;

namespace FoodLovera.Core.Services;

public interface IUserService
{
    Task<MyProfileResponseDTO> GetMyProfileAsync(int userId, CancellationToken ct);
    Task ChangeUsernameAsync(int userId, ChangeUsernameRequestDTO request, CancellationToken ct);
}