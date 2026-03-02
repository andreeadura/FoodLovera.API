using FoodLovera.Core.Abstractions;
using FoodLovera.Models.Entities;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class SessionParticipantRepository : ISessionParticipantRepository
{
    private readonly FoodLoveraDbContext _db;

    public SessionParticipantRepository(FoodLoveraDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(SessionParticipant participant, CancellationToken ct)
        => _db.SessionParticipants.AddAsync(participant, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}