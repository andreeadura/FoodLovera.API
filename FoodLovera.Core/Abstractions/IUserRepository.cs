#nullable enable
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}