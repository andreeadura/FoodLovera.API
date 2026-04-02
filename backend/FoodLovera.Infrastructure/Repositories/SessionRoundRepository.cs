using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class SessionRoundRepository : ISessionRoundRepository
{
    private readonly FoodLoveraDbContext _db;

    public SessionRoundRepository(FoodLoveraDbContext db)
    {
        _db = db;
    }

    public Task<SessionRound?> GetOpenRoundAsync(int sessionId, CancellationToken ct)
        => _db.SessionRounds
            .Include(x => x.Restaurant)
                .ThenInclude(r => r.RestaurantCategories)
                    .ThenInclude(rc => rc.Category)
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && !x.IsClosed, ct);

    public Task<SessionRound?> GetLatestRoundAsync(int sessionId, CancellationToken ct)
        => _db.SessionRounds
            .Include(x => x.Restaurant)
                .ThenInclude(r => r.RestaurantCategories)
                    .ThenInclude(rc => rc.Category)
            .Include(x => x.Votes)
            .Where(x => x.SessionId == sessionId)
            .OrderByDescending(x => x.RoundNumber)
            .FirstOrDefaultAsync(ct);

    public Task AddAsync(SessionRound round, CancellationToken ct)
        => _db.SessionRounds.AddAsync(round, ct).AsTask();

    public Task<SessionRoundVote?> GetVoteAsync(int sessionRoundId, int participantId, CancellationToken ct)
        => _db.SessionRoundVotes
            .FirstOrDefaultAsync(x => x.SessionRoundId == sessionRoundId && x.ParticipantId == participantId, ct);

    public Task AddVoteAsync(SessionRoundVote vote, CancellationToken ct)
        => _db.SessionRoundVotes.AddAsync(vote, ct).AsTask();

    public Task<List<SessionRoundVote>> GetVotesAsync(int sessionRoundId, CancellationToken ct)
        => _db.SessionRoundVotes
            .Where(x => x.SessionRoundId == sessionRoundId)
            .ToListAsync(ct);

    public Task<List<int>> GetShownRestaurantIdsAsync(int sessionId, CancellationToken ct)
        => _db.SessionRounds
            .Where(x => x.SessionId == sessionId)
            .OrderBy(x => x.RoundNumber)
            .Select(x => x.RestaurantId)
            .ToListAsync(ct);
}