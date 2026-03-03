using FoodLovera.Core.Contracts;
using FoodLovera.Core.Helpers;
using FoodLovera.Core.Exceptions;
using FoodLovera.Models.Models;

namespace FoodLovera.Core.Services;

public sealed class UserService(IUserRepository users, IUnitOfWork uow) : IUserService
{
    private readonly IUserRepository _users = users;
    private readonly IUnitOfWork _uow = uow;

    public async Task ChangeUsernameAsync(int userId, ChangeUsernameRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (userId <= 0) throw new ArgumentException("Invalid user id.", nameof(userId));

        var normalized = UsernameHelper.Normalize(request.Username);
        UsernameHelper.ValidateOrThrow(normalized, nameof(request.Username));

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (string.Equals(user.Username, normalized, StringComparison.Ordinal))
            return;

        var taken = await _users.UsernameExistsAsync(normalized, excludeUserId: userId, ct);
        if (taken)
            throw new ConflictException("Username already taken.");

        user.Username = normalized;

        await _uow.SaveChangesAsync(ct);
    }
}