#nullable enable
namespace FoodLovera.Models.Models;

public sealed record RegisterRequest(string Email, string Password, string Username); 
public sealed record LoginRequest(string Email, string Password);
public sealed record AuthResponse(string? AccessToken, bool RequiresEmailVerification);
