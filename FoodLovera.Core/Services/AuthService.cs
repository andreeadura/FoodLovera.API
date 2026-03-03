#nullable enable

using FoodLovera.Core.Contracts;
using FoodLovera.Core.Exceptions;
using FoodLovera.Core.Helpers;
using FoodLovera.Models.Entities;
using FoodLovera.Models.Models;
using System.Security.Authentication;
using AuthenticationException = FoodLovera.Core.Exceptions.AuthenticationException;

namespace FoodLovera.Core.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;

    public AuthService(IUserRepository users, IUnitOfWork uow, IJwtTokenService jwt)
    {
        _users = users;
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException("Email is required.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Password)) throw new ArgumentException("Password is required.", nameof(request.Password));

        var email = request.Email.Trim().ToLowerInvariant();

        var existing = await _users.GetByEmailAsync(email, ct);
        if (existing is not null)
            throw new ConflictException("Email already registered.");

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse(_jwt.CreateAccessToken(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException("Email is required.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Password)) throw new ArgumentException("Password is required.", nameof(request.Password));

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            throw new AuthenticationException("Invalid credentials.");

        return new AuthResponse(_jwt.CreateAccessToken(user));
    }
}