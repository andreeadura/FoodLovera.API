using FoodLovera.Core.Abstractions;
using FoodLovera.Core.Contracts;
using FoodLovera.Core.Exceptions;
using FoodLovera.Core.Helpers;
using FoodLovera.Models.Entities;
using FoodLovera.Models.Enums;
using System.Security.Cryptography;

namespace FoodLovera.Core.Services;

public sealed class SessionService : ISessionService
{
    private readonly ISessionRepository _sessions;
    private readonly ISessionParticipantRepository _participants;
    private readonly IParticipantRestaurantActionRepository _actions;
    private readonly IRestaurantRepository _restaurants;
    private readonly ICityRepository _cities;
    private readonly ICategoryRepository _categories;
    private readonly IGeocodingService _geocoding;

    public SessionService(
        ISessionRepository sessions,
        ISessionParticipantRepository participants,
        IParticipantRestaurantActionRepository actions,
        IRestaurantRepository restaurants,
        ICityRepository cities,
        ICategoryRepository categories,
        IGeocodingService geocoding)

    {
        _sessions = sessions;
        _participants = participants;
        _actions = actions;
        _restaurants = restaurants;
        _cities = cities;
        _categories = categories;
        _geocoding = geocoding;
    }

    public async Task<CreateSessionResponse> CreateAsync(CreateSessionRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.", nameof(request));

        var joinCode = await GenerateUniqueJoinCodeAsync(ct);

        Guid selectedCityId;

        if (request.SelectedCityId is Guid providedCityId)
        {
            if (!await _cities.ExistsAsync(providedCityId, ct))
                throw new ArgumentException("SelectedCityId does not exist.", nameof(request.SelectedCityId));

            selectedCityId = providedCityId;
        }
        else
        {
           
            string? detectedCityName = null;

            if (request.Latitude is double lat && request.Longitude is double lng)
                detectedCityName = await _geocoding.ReverseGeocodeCityAsync(lat, lng, ct);

            if (!string.IsNullOrWhiteSpace(detectedCityName))
            {
                var detectedKey = CityNameNormalizer.ToCityKey(detectedCityName);

                selectedCityId =
                    await _cities.GetIdByCityKeyAsync(detectedKey, ct)
                    ?? await _cities.GetIdByCityKeyAsync(CityNameNormalizer.ToCityKey("Cluj-Napoca"), ct)
                    ?? throw new InvalidOperationException("Default city 'Cluj-Napoca' not found in database.");
            }
            else
            {
                selectedCityId =
                    await _cities.GetIdByCityKeyAsync(CityNameNormalizer.ToCityKey("Cluj-Napoca"), ct)
                    ?? throw new InvalidOperationException("Default city 'Cluj-Napoca' not found in database.");
            }
        }

        var useAllCategories = request.UseAllCategories;
        var categoryIds = (request.CategoryIds ?? Array.Empty<Guid>()).Distinct().ToList();

        if (!useAllCategories && categoryIds.Count == 0)
            throw new ArgumentException(
                "When UseAllCategories is false, CategoryIds must be provided.",
                nameof(request.CategoryIds));

        if (!useAllCategories)
        {
            var existing = await _categories.GetExistingIdsAsync(categoryIds, ct);
            if (existing.Count != categoryIds.Count)
                throw new ArgumentException("One or more CategoryIds do not exist.", nameof(request.CategoryIds));
        }

        var session = new Session
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            JoinCode = joinCode,
            CreatedAt = DateTime.UtcNow,

            IsActive = true,
            Status = SessionStatus.Active,

            CompletedReason = null,
            CompletedAt = null,

            SelectedCityId = selectedCityId,
            UseAllCategories = useAllCategories
        };

        if (!useAllCategories)
        {
            session.SessionCategories = categoryIds
                .Select(id => new SessionCategory
                {
                    SessionId = session.Id,
                    CategoryId = id
                })
                .ToList();
        }

        await _sessions.AddAsync(session, ct);
        await _sessions.SaveChangesAsync(ct);

        return new CreateSessionResponse
        {
            SessionId = session.Id,
            JoinCode = session.JoinCode
        };
    }

    public async Task<JoinSessionResponse> JoinAsync(string joinCode, JoinSessionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
            throw new ArgumentException("JoinCode is required.", nameof(joinCode));

        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new ArgumentException("Participant name is required.", nameof(request));

        var session = await _sessions.GetByJoinCodeAsync(joinCode.Trim(), ct);
        if (session is null)
            throw new NotFoundException("Session not found.");

        if (!session.IsActive || session.Status == SessionStatus.Completed)
            throw new InvalidOperationException("Session is not active.");

        var participant = new SessionParticipant
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            Name = request.DisplayName.Trim(),
            IsActive = true
        };

        await _participants.AddAsync(participant, ct);
        await _participants.SaveChangesAsync(ct);

        return new JoinSessionResponse
        {
            SessionId = session.Id,
            ParticipantId = participant.Id
        };
    }

    public async Task<NextResponse> NextAsync(Guid sessionId, NextRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.ParticipantId == Guid.Empty)
            throw new ArgumentException("ParticipantId is required.", nameof(request));

        var session = await _sessions.GetByIdAsync(sessionId, ct);
        if (session is null)
            throw new NotFoundException("Session not found.");

        if (!session.IsActive || session.Status == SessionStatus.Completed)
        {
            var completedWinners = await GetCompletedSessionWinnersAsync(sessionId, session.CompletedReason, ct);

            return new NextResponse
            {
                Completed = true,
                CurrentRestaurantId = null,
                Winners = completedWinners
            };
        }

        var participantOk = await _participants.ExistsInSessionAsync(sessionId, request.ParticipantId, ct);
        if (!participantOk)
            throw new InvalidOperationException("Participant not in this session.");

        var participant = await _participants.GetByIdAsync(request.ParticipantId, ct);
        if (participant is null || participant.SessionId != sessionId)
            throw new InvalidOperationException("Participant not found.");

        if (participant.IsFinished)
        {
            return new NextResponse
            {
                Completed = false,
                CurrentRestaurantId = null,
                Winners = new List<WinnerResponse>()
            };
        }

        if (participant.CurrentRestaurantId is Guid currentRestaurantId)
            await _actions.AddSeenIfMissingAsync(sessionId, participant.Id, currentRestaurantId, ct);

        var actedIds = await _actions.GetActedRestaurantIdsAsync(sessionId, participant.Id, ct);

        var selectedCategoryIds = session.UseAllCategories
            ? Array.Empty<Guid>()
            : session.SessionCategories
                .Select(sc => sc.CategoryId)
                .Distinct()
                .ToArray();

        var nextRestaurantId = await _restaurants.GetNextRestaurantIdAsync(
            selectedCityId: session.SelectedCityId,
            useAllCategories: session.UseAllCategories,
            selectedCategoryIds: selectedCategoryIds,
            excludedRestaurantIds: actedIds,
            ct: ct);

        if (nextRestaurantId is Guid nextId)
        {
            participant.CurrentRestaurantId = nextId;
            await _participants.SaveChangesAsync(ct);

            return new NextResponse
            {
                Completed = false,
                CurrentRestaurantId = nextId,
                Winners = new List<WinnerResponse>()
            };
        }

        participant.IsFinished = true;
        participant.CurrentRestaurantId = null;
        await _participants.SaveChangesAsync(ct);

        var remaining = await _participants.CountNotFinishedAsync(sessionId, ct);
        if (remaining == 0)
        {
            var winnerIds = await ComputeWinnerIdsByTopLikesAsync(sessionId, ct);

            session.Status = SessionStatus.Completed;
            session.IsActive = false;
            session.CompletedReason = SessionCompletedReason.DeckExhausted;
            session.CompletedAt = DateTime.UtcNow;

            await _sessions.SaveChangesAsync(ct);

            var winners = await BuildWinnersAsync(winnerIds, SessionCompletedReason.DeckExhausted, ct);

            return new NextResponse
            {
                Completed = true,
                CurrentRestaurantId = null,
                Winners = winners
            };
        }

        return new NextResponse
        {
            Completed = false,
            CurrentRestaurantId = null,
            Winners = new List<WinnerResponse>()
        };
    }

    public async Task<LikeResponse> LikeAsync(Guid sessionId, Guid restaurantId, LikeRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.ParticipantId == Guid.Empty)
            throw new ArgumentException("ParticipantId is required.", nameof(request));
        if (restaurantId == Guid.Empty)
            throw new ArgumentException("RestaurantId is required.", nameof(restaurantId));

        var session = await _sessions.GetByIdAsync(sessionId, ct);
        if (session is null)

            throw new NotFoundException("Session not found.");
        if (!session.IsActive || session.Status == SessionStatus.Completed)
        {
            var completedWinners = await GetCompletedSessionWinnersAsync(sessionId, session.CompletedReason, ct);
            return new LikeResponse
            {
                Completed = true,
                CurrentRestaurantId = null,
                Winners = completedWinners
            };
        }

        var participant = await _participants.GetByIdAsync(request.ParticipantId, ct);
        if (participant is null || participant.SessionId != sessionId || !participant.IsActive)
            throw new InvalidOperationException("Participant not in this session.");

        if (participant.IsFinished)
            throw new InvalidOperationException("Participant has finished the deck.");

        if (participant.CurrentRestaurantId is not Guid currentId || currentId != restaurantId)
            throw new InvalidOperationException("You can only like your current restaurant.");

        await _actions.SetLikedAsync(sessionId, participant.Id, restaurantId, ct);

        var likeCount = await _actions.GetLikeCountForRestaurantAsync(sessionId, restaurantId, ct);
        var participantCount = await _participants.CountActiveAsync(sessionId, ct);

        if (participantCount > 0 && likeCount == participantCount)
        {
            session.Status = SessionStatus.Completed;
            session.IsActive = false;
            session.CompletedReason = SessionCompletedReason.UnanimousMatch;
            session.CompletedAt = DateTime.UtcNow;

            await _sessions.SaveChangesAsync(ct);

            var winners = await BuildWinnersAsync(
                new List<Guid> { restaurantId },
                SessionCompletedReason.UnanimousMatch,
                ct);

            return new LikeResponse
            {
                Completed = true,
                CurrentRestaurantId = null,
                Winners = winners
            };
        }

        return new LikeResponse
        {
            Completed = false,
            CurrentRestaurantId = participant.CurrentRestaurantId,
            Winners = new List<WinnerResponse>()
        };
    }

    private async Task<List<WinnerResponse>> GetCompletedSessionWinnersAsync(
        Guid sessionId,
        SessionCompletedReason? completedReason,
        CancellationToken ct)
    {
        if (completedReason is null)
            return new List<WinnerResponse>();

        return completedReason.Value switch
        {
            SessionCompletedReason.UnanimousMatch => await GetMatchWinnersAsync(sessionId, ct),
            SessionCompletedReason.DeckExhausted => await GetTopLikesWinnersAsync(sessionId, ct),
            _ => new List<WinnerResponse>()
        };
    }

    private async Task<List<WinnerResponse>> GetMatchWinnersAsync(Guid sessionId, CancellationToken ct)
    {
        var participantCount = await _participants.CountActiveAsync(sessionId, ct);
        if (participantCount <= 0) return new List<WinnerResponse>();

        var likeCounts = await _actions.GetLikeCountsByRestaurantAsync(sessionId, ct);

        var matchIds = likeCounts
            .Where(kvp => kvp.Value == participantCount)
            .Select(kvp => kvp.Key)
            .ToList();

        return await BuildWinnersAsync(matchIds, SessionCompletedReason.UnanimousMatch, ct);
    }

    private async Task<List<WinnerResponse>> GetTopLikesWinnersAsync(Guid sessionId, CancellationToken ct)
    {
        var ids = await ComputeWinnerIdsByTopLikesAsync(sessionId, ct);
        return await BuildWinnersAsync(ids, SessionCompletedReason.DeckExhausted, ct);
    }

    private async Task<List<Guid>> ComputeWinnerIdsByTopLikesAsync(Guid sessionId, CancellationToken ct)
    {
        var likeCounts = await _actions.GetLikeCountsByRestaurantAsync(sessionId, ct);
        if (likeCounts.Count == 0) return new List<Guid>();

        var max = likeCounts.Values.Max();
        return likeCounts.Where(x => x.Value == max).Select(x => x.Key).ToList();
    }

    private async Task<List<WinnerResponse>> BuildWinnersAsync(
        IReadOnlyList<Guid> winnerIds,
        SessionCompletedReason reason,
        CancellationToken ct)
    {
        if (winnerIds.Count == 0)
            return new List<WinnerResponse>();

        var names = await _restaurants.GetNamesByIdsAsync(winnerIds, ct);

        return winnerIds.Select(id => new WinnerResponse
        {
            RestaurantId = id,
            RestaurantName = names.TryGetValue(id, out var name) ? name : "(unknown)",
            Reason = reason
        }).ToList();
    }

    private async Task<string> GenerateUniqueJoinCodeAsync(CancellationToken ct)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ23456789";
        var bytes = new byte[6];

        for (var attempt = 0; attempt < 30; attempt++)
        {
            RandomNumberGenerator.Fill(bytes);

            var chars = new char[6];
            for (var i = 0; i < chars.Length; i++)
                chars[i] = alphabet[bytes[i] % alphabet.Length];

            var code = new string(chars);

            if (!await _sessions.JoinCodeExistsAsync(code, ct))
                return code;
        }

        throw new InvalidOperationException("Could not generate a unique join code. Try again.");
    }
}