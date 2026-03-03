using FoodLovera.Models.Enums;

namespace FoodLovera.Core.Contracts;

public interface IParticipantRestaurantActionRepository
{
    Task<HashSet<int>> GetActedRestaurantIdsAsync(int sessionId, int participantId, CancellationToken ct);

    Task<ParticipantRestaurantActionType?> GetActionTypeAsync(int sessionId, int participantId, int restaurantId, CancellationToken ct);

    Task AddSeenIfMissingAsync(int sessionId, int participantId, int restaurantId, CancellationToken ct);

    Task<Dictionary<int, int>> GetLikeCountsByRestaurantAsync(int sessionId, CancellationToken ct);

    Task SetLikedAsync(int sessionId, int participantId, int restaurantId, CancellationToken ct);

    Task<int> GetLikeCountForRestaurantAsync(int sessionId, int restaurantId, CancellationToken ct);
}