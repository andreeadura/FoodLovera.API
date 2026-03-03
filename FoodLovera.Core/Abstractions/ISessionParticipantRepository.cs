using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Abstractions;

public interface ISessionParticipantRepository
{
    Task AddAsync(SessionParticipant participant, CancellationToken ct);
  
    Task<bool> ExistsInSessionAsync(Guid sessionId, Guid participantId, CancellationToken ct);
    Task<int> CountActiveAsync(Guid sessionId, CancellationToken ct);
    Task<SessionParticipant?> GetByIdAsync(Guid participantId, CancellationToken ct);
    Task<int> CountNotFinishedAsync(Guid sessionId, CancellationToken ct);
    Task MarkFinishedAsync(Guid participantId, CancellationToken ct);
}