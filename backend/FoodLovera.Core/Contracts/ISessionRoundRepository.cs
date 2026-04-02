using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface ISessionRoundRepository
{
    Task<SessionRound?> GetOpenRoundAsync(int sessionId, CancellationToken ct);
    Task<SessionRound?> GetLatestRoundAsync(int sessionId, CancellationToken ct);

    Task AddAsync(SessionRound round, CancellationToken ct);

    Task<SessionRoundVote?> GetVoteAsync(int sessionRoundId, int participantId, CancellationToken ct);
    Task AddVoteAsync(SessionRoundVote vote, CancellationToken ct);

    Task<List<SessionRoundVote>> GetVotesAsync(int sessionRoundId, CancellationToken ct);
    Task<List<int>> GetShownRestaurantIdsAsync(int sessionId, CancellationToken ct);
}