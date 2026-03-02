using FoodLovera.Models.Enums;

namespace FoodLovera.Core.Abstractions;

public interface IParticipantRestaurantActionRepository
{
    Task<HashSet<Guid>> GetActedRestaurantIdsAsync(Guid sessionId, Guid participantId, CancellationToken ct);

    Task<ParticipantRestaurantActionType?> GetActionTypeAsync(Guid sessionId, Guid participantId, Guid restaurantId, CancellationToken ct);

    Task AddSeenIfMissingAsync(Guid sessionId, Guid participantId, Guid restaurantId, CancellationToken ct);

    Task<Dictionary<Guid, int>> GetLikeCountsByRestaurantAsync(Guid sessionId, CancellationToken ct);

    Task SetLikedAsync(Guid sessionId, Guid participantId, Guid restaurantId, CancellationToken ct);

    Task<int> GetLikeCountForRestaurantAsync(Guid sessionId, Guid restaurantId, CancellationToken ct);
}