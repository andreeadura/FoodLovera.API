#nullable enable
using FoodLovera;
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}