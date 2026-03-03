using Microsoft.EntityFrameworkCore;
using FoodLovera.Core.Abstractions;
using FoodLovera.Models.Entities;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class SessionRepository : ISessionRepository
{
    private readonly FoodLoveraDbContext _db;

    public SessionRepository(FoodLoveraDbContext db) => _db = db;

    public Task<bool> JoinCodeExistsAsync(string joinCode, CancellationToken ct)
        => _db.Sessions.AnyAsync(s => s.JoinCode == joinCode, ct);

    public Task<Session?> GetByJoinCodeAsync(string joinCode, CancellationToken ct)
        => _db.Sessions.FirstOrDefaultAsync(s => s.JoinCode == joinCode, ct);

    public async Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken ct)
    {
        return await _db.Sessions
            .Include(s => s.SessionCategories) 
            .FirstOrDefaultAsync(s => s.Id == sessionId, ct);
    }

    public Task AddAsync(Session session, CancellationToken ct)
        => _db.Sessions.AddAsync(session, ct).AsTask();


  
}