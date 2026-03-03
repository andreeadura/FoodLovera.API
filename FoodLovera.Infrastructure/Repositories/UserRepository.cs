#nullable enable
using FoodLovera.Core.Abstractions;
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
}