using FoodLovera.Core.Abstractions;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class SessionParticipantRepository : ISessionParticipantRepository
{
    private readonly FoodLoveraDbContext _db;

    public SessionParticipantRepository(FoodLoveraDbContext db) => _db = db;

    public Task AddAsync(SessionParticipant participant, CancellationToken ct)
        => _db.SessionParticipants.AddAsync(participant, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);

    public Task<bool> ExistsInSessionAsync(Guid sessionId, Guid participantId, CancellationToken ct)
        => _db.SessionParticipants.AnyAsync(p => p.SessionId == sessionId && p.Id == participantId && p.IsActive, ct);

    public Task<int> CountActiveAsync(Guid sessionId, CancellationToken ct)
        => _db.SessionParticipants.CountAsync(p => p.SessionId == sessionId && p.IsActive, ct);
    public Task<SessionParticipant?> GetByIdAsync(Guid participantId, CancellationToken ct)
    => _db.SessionParticipants.FirstOrDefaultAsync(p => p.Id == participantId, ct);

    public Task<int> CountNotFinishedAsync(Guid sessionId, CancellationToken ct)
        => _db.SessionParticipants.AsNoTracking()
            .CountAsync(p => p.SessionId == sessionId && p.IsActive && !p.IsFinished, ct);

    public async Task MarkFinishedAsync(Guid participantId, CancellationToken ct)
    {
        var p = await _db.SessionParticipants.FirstOrDefaultAsync(x => x.Id == participantId, ct);
        if (p is null) return;

        if (!p.IsFinished)
        {
            p.IsFinished = true;
            p.CurrentRestaurantId = null;
            await _db.SaveChangesAsync(ct);
        }
    }
}