using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Abstractions;

public interface ISessionParticipantRepository
{
    Task AddAsync(SessionParticipant participant, CancellationToken ct);
  
    Task<bool> ExistsInSessionAsync(int sessionId, int participantId, CancellationToken ct);
    Task<int> CountActiveAsync(int sessionId, CancellationToken ct);
    Task<SessionParticipant?> GetByIdAsync(int participantId, CancellationToken ct);
    Task<int> CountNotFinishedAsync(int sessionId, CancellationToken ct);
    Task MarkFinishedAsync(int participantId, CancellationToken ct);
}