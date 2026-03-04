using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;
using FoodLovera.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class ParticipantRestaurantActionRepository : IParticipantRestaurantActionRepository
{
    private readonly FoodLoveraDbContext _db;

    public ParticipantRestaurantActionRepository(FoodLoveraDbContext db) => _db = db;

    public async Task<HashSet<int>> GetActedRestaurantIdsAsync(int sessionId, int participantId, CancellationToken ct)
    {
        var ids = await _db.ParticipantRestaurantActions.AsNoTracking()
            .Where(a => a.SessionId == sessionId && a.ParticipantId == participantId)
            .Select(a => a.RestaurantId)
            .ToListAsync(ct);

        return ids.ToHashSet();
    }

    public async Task<ParticipantRestaurantActionType?> GetActionTypeAsync(int sessionId, int participantId, int restaurantId, CancellationToken ct)
    {
        return await _db.ParticipantRestaurantActions.AsNoTracking()
            .Where(a => a.SessionId == sessionId && a.ParticipantId == participantId && a.RestaurantId == restaurantId)
            .Select(a => (ParticipantRestaurantActionType?)a.ActionType)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddSeenIfMissingAsync(int sessionId, int participantId, int restaurantId, CancellationToken ct)
    {
        var existing = await _db.ParticipantRestaurantActions
            .FirstOrDefaultAsync(a => a.SessionId == sessionId
                                   && a.ParticipantId == participantId
                                   && a.RestaurantId == restaurantId, ct);

        if (existing is not null)
            return;

        _db.ParticipantRestaurantActions.Add(new ParticipantRestaurantAction
        {
            
            SessionId = sessionId,
            ParticipantId = participantId,
            RestaurantId = restaurantId,
            ActionType = ParticipantRestaurantActionType.Seen,
            CreatedAt = DateTime.UtcNow
        });

        
    }

    public async Task<Dictionary<int, int>> GetLikeCountsByRestaurantAsync(int sessionId, CancellationToken ct)
    {
        var rows = await _db.ParticipantRestaurantActions.AsNoTracking()
            .Where(a => a.SessionId == sessionId && a.ActionType == ParticipantRestaurantActionType.Liked)
            .GroupBy(a => a.RestaurantId)
            .Select(g => new { RestaurantId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return rows.ToDictionary(x => x.RestaurantId, x => x.Count);
    }
    public async Task SetLikedAsync(int sessionId, int participantId, int restaurantId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var existing = await _db.ParticipantRestaurantActions
            .FirstOrDefaultAsync(a => a.SessionId == sessionId
                                   && a.ParticipantId == participantId
                                   && a.RestaurantId == restaurantId, ct);

        if (existing is null)
        {
            _db.ParticipantRestaurantActions.Add(new ParticipantRestaurantAction
            {
                
                SessionId = sessionId,
                ParticipantId = participantId,
                RestaurantId = restaurantId,
                ActionType = ParticipantRestaurantActionType.Liked,
                CreatedAt = now
            });
            return;
        }

        if (existing.ActionType == ParticipantRestaurantActionType.Liked)
            return;

        existing.ActionType = ParticipantRestaurantActionType.Liked;
        existing.CreatedAt = now;
    }

    public Task<int> GetLikeCountForRestaurantAsync(int sessionId, int restaurantId, CancellationToken ct)
    {
        return _db.ParticipantRestaurantActions.AsNoTracking()
            .CountAsync(a => a.SessionId == sessionId
                          && a.RestaurantId == restaurantId
                          && a.ActionType == ParticipantRestaurantActionType.Liked, ct);
    }
}