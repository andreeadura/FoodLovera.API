using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Abstractions;

public interface ISessionParticipantRepository
{
    Task AddAsync(SessionParticipant participant, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}