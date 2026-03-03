using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Abstractions;

public interface ISessionRepository
{
    Task<bool> JoinCodeExistsAsync(string joinCode, CancellationToken ct);
    Task AddAsync(Session session, CancellationToken ct);
   
    Task<Session?> GetByJoinCodeAsync(string joinCode, CancellationToken ct);
    Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken ct);
}