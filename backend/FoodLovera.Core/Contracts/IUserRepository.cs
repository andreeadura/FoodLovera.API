#nullable enable
using FoodLovera;
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    void Remove(User user);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct);
    Task<bool> UsernameExistsAsync(string username, int? excludeUserId, CancellationToken ct);

}