#nullable enable


using FoodLovera.Models.Models;

namespace FoodLovera.Core.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task VerifyEmailAsync(VerifyEmailRequestDTO request, CancellationToken ct);
    Task ResendVerificationCodeAsync(string email, CancellationToken ct);
}