#nullable enable
using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly FoodLoveraDbContext _db;
    public UserRepository(FoodLoveraDbContext db) => _db = db;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct) =>
        await _db.Users.AddAsync(user, ct);

    public Task<User?> GetByIdAsync(int id, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);

    public void Remove(User user) => _db.Users.Remove(user);
    public Task<bool> UsernameExistsAsync(string username, int? excludeUserId, CancellationToken ct) =>
    _db.Users.AnyAsync(x => x.Username == username && (excludeUserId == null || x.Id != excludeUserId.Value), ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct) =>
    await _db.Users
        .AsNoTracking()
        .OrderBy(u => u.Email)
        .ToListAsync(ct);

}