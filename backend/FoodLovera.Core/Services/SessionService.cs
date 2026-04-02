#nullable enable

using FoodLovera.Core.Contracts;
using FoodLovera.Core.Exceptions;
using FoodLovera.Core.Helpers;
using FoodLovera.Models.Entities;
using FoodLovera.Models.Enums;
using FoodLovera.Models.Models;
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
    private readonly ISessionRoundRepository _rounds;
    private readonly IUnitOfWork _uow;

    public SessionService(
        ISessionRepository sessions,
        ISessionParticipantRepository participants,
        IParticipantRestaurantActionRepository actions,
        IRestaurantRepository restaurants,
        ICityRepository cities,
        ICategoryRepository categories,
        IGeocodingService geocoding,
        ISessionRoundRepository rounds,
        IUnitOfWork uow)
    {
        _sessions = sessions;
        _participants = participants;
        _actions = actions;
        _restaurants = restaurants;
        _cities = cities;
        _categories = categories;
        _geocoding = geocoding;
        _rounds = rounds;
        _uow = uow;
    }
    private async Task<CurrentRestaurantDTO?> BuildCurrentRestaurantAsync(int? restaurantId, CancellationToken ct)
    {
        if (restaurantId is not int id)
            return null;

        var restaurant = await _restaurants.GetByIdWithCategoriesAsync(id, ct);
        if (restaurant is null)
            return null;

        return new CurrentRestaurantDTO
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            ImageUrl = restaurant.ImageUrl,
            Categories = restaurant.RestaurantCategories
                .Select(rc => rc.Category.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .OrderBy(name => name)
                .ToList()
        };
    }
    public async Task<CreateSessionResponseDTO> CreateAsync(CreateSessionRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.", nameof(request));

        var joinCode = await GenerateUniqueJoinCodeAsync(ct);

        int selectedCityId;

        if (request.SelectedCityId is int providedCityId)
        {
            if (providedCityId <= 0)
                throw new ArgumentException("SelectedCityId must be > 0.", nameof(request.SelectedCityId));

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
        var categoryIds = (request.CategoryIds ?? Array.Empty<int>())
            .Where(x => x > 0)
            .Distinct()
            .ToList();

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

        if (request.RequiredParticipants is < 2 or > 20)
            throw new ArgumentException("RequiredParticipants must be between 2 and 20.", nameof(request.RequiredParticipants));

        var session = new Session
        {
            Name = request.Name.Trim(),
            JoinCode = joinCode,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Status = SessionStatus.Active,
            CompletedReason = null,
            CompletedAt = null,
            SelectedCityId = selectedCityId,
            UseAllCategories = useAllCategories,
            RequiredParticipants = request.RequiredParticipants
        };

        if (!useAllCategories)
        {
            session.SessionCategories = categoryIds
                .Select(id => new SessionCategory
                {
                    Session = session,
                    CategoryId = id
                })
                .ToList();
        }

        await _sessions.AddAsync(session, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreateSessionResponseDTO
        {
            SessionId = session.Id,
            JoinCode = session.JoinCode
        };
    }

    public async Task<JoinSessionResponseDTO> JoinAsync(string joinCode, JoinSessionRequestDTO request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
            throw new ArgumentException("JoinCode is required.", nameof(joinCode));

        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new ArgumentException("Participant name is required.", nameof(request.DisplayName));

        var session = await _sessions.GetByJoinCodeAsync(joinCode.Trim(), ct);
        if (session is null)
            throw new InvalidOperationException("Session not found.");

        if (!session.IsActive || session.Status == SessionStatus.Completed)
            throw new InvalidOperationException("Session is not active.");

        var participant = new SessionParticipant
        {
            SessionId = session.Id,
            Name = request.DisplayName.Trim(),
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            IsFinished = false,
            CurrentRestaurantId = null
        };

        await _participants.AddAsync(participant, ct);
        await _uow.SaveChangesAsync(ct);

        return new JoinSessionResponseDTO
        {
            SessionId = session.Id,
            ParticipantId = participant.Id
        };
    }

    // Legacy endpoints left as-is
    public async Task<NextResponseDTO> NextAsync(int sessionId, NextRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.ParticipantId <= 0)
            throw new ArgumentException("ParticipantId is required.", nameof(request.ParticipantId));
        if (sessionId <= 0)
            throw new ArgumentException("SessionId is required.", nameof(sessionId));

        var session = await _sessions.GetByIdAsync(sessionId, ct);
        if (session is null)
            throw new InvalidOperationException("Session not found.");

        var current = await _participants.CountBySessionIdAsync(sessionId, ct);
        if (current < session.RequiredParticipants)
            throw new ConflictException($"Not enough participants. Required {session.RequiredParticipants}, current {current}.");

        if (!session.IsActive || session.Status == SessionStatus.Completed)
        {
            var completedWinners = await GetCompletedSessionWinnersAsync(sessionId, session.CompletedReason, ct);

            return new NextResponseDTO
            {
                Completed = true,
                CurrentRestaurant = null,
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
            return new NextResponseDTO
            {
                Completed = false,
                CurrentRestaurant = null,
                Winners = new List<WinnerResponseDTO>()
            };
        }

        if (participant.CurrentRestaurantId is int currentRestaurantId)
            await _actions.AddSeenIfMissingAsync(sessionId, participant.Id, currentRestaurantId, ct);

        await _uow.SaveChangesAsync(ct);

        var actedIds = await _actions.GetActedRestaurantIdsAsync(sessionId, participant.Id, ct);

        var selectedCategoryIds = session.UseAllCategories
            ? Array.Empty<int>()
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

        if (nextRestaurantId is int nextId)
        {
            participant.CurrentRestaurantId = nextId;
            await _uow.SaveChangesAsync(ct);

            var currentRestaurant = await BuildCurrentRestaurantAsync(nextId, ct);

            return new NextResponseDTO
            {
                Completed = false,
                CurrentRestaurant = currentRestaurant,
                Winners = new List<WinnerResponseDTO>()
            };
        }

        participant.IsFinished = true;
        participant.CurrentRestaurantId = null;
        await _uow.SaveChangesAsync(ct);

        var remaining = await _participants.CountNotFinishedAsync(sessionId, ct);
        if (remaining == 0)
        {
            var winnerIds = await ComputeWinnerIdsByTopLikesAsync(sessionId, ct);

            session.Status = SessionStatus.Completed;
            session.IsActive = false;
            session.CompletedReason = SessionCompletedReason.DeckExhausted;
            session.CompletedAt = DateTime.UtcNow;

            await _uow.SaveChangesAsync(ct);

            var winners = await BuildWinnersAsync(winnerIds, SessionCompletedReason.DeckExhausted, ct);

            return new NextResponseDTO
            {
                Completed = true,
                CurrentRestaurant = null,
                Winners = winners
            };
        }

        return new NextResponseDTO
        {
            Completed = false,
            CurrentRestaurant = null,
            Winners = new List<WinnerResponseDTO>()
        };
    }

    public async Task<LikeResponseDTO> LikeAsync(int sessionId, int restaurantId, LikeRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.ParticipantId <= 0)
            throw new ArgumentException("ParticipantId is required.", nameof(request.ParticipantId));
        if (restaurantId <= 0)
            throw new ArgumentException("RestaurantId is required.", nameof(restaurantId));
        if (sessionId <= 0)
            throw new ArgumentException("SessionId is required.", nameof(sessionId));

        var session = await _sessions.GetByIdAsync(sessionId, ct);
        if (session is null)
            throw new InvalidOperationException("Session not found.");

        if (!session.IsActive || session.Status == SessionStatus.Completed)
        {
            var completedWinners = await GetCompletedSessionWinnersAsync(sessionId, session.CompletedReason, ct);
            return new LikeResponseDTO
            {
                Completed = true,
                CurrentRestaurant = null,
                Winners = completedWinners
            };
        }

        var participant = await _participants.GetByIdAsync(request.ParticipantId, ct);
        if (participant is null || participant.SessionId != sessionId || !participant.IsActive)
            throw new InvalidOperationException("Participant not in this session.");

        if (participant.IsFinished)
            throw new InvalidOperationException("Participant has finished the deck.");

        if (participant.CurrentRestaurantId is not int currentId || currentId != restaurantId)
            throw new InvalidOperationException("You can only like your current restaurant.");

        await _actions.SetLikedAsync(sessionId, participant.Id, restaurantId, ct);
        await _uow.SaveChangesAsync(ct);

        var likeCount = await _actions.GetLikeCountForRestaurantAsync(sessionId, restaurantId, ct);
        var participantCount = await _participants.CountActiveAsync(sessionId, ct);

        if (participantCount > 0 && likeCount == participantCount)
        {
            session.Status = SessionStatus.Completed;
            session.IsActive = false;
            session.CompletedReason = SessionCompletedReason.UnanimousMatch;
            session.CompletedAt = DateTime.UtcNow;

            await _uow.SaveChangesAsync(ct);

            var winners = await BuildWinnersAsync(
                new List<int> { restaurantId },
                SessionCompletedReason.UnanimousMatch,
                ct);

            return new LikeResponseDTO
            {
                Completed = true,
                CurrentRestaurant = null,
                Winners = winners
            };
        }

        var currentRestaurant = await BuildCurrentRestaurantAsync(participant.CurrentRestaurantId, ct);

        return new LikeResponseDTO
        {
            Completed = false,
            CurrentRestaurant = currentRestaurant,
            Winners = new List<WinnerResponseDTO>()
        };
    }

    public async Task<SessionStatusResponseDTO> GetStatusAsync(int sessionId, CancellationToken ct)
    {
        var session = await _sessions.GetByIdAsync(sessionId, ct);
        if (session is null)
            throw new NotFoundException("Session not found.");

        var current = await _participants.CountBySessionIdAsync(sessionId, ct);

        return new SessionStatusResponseDTO
        {
            SessionId = session.Id,
            JoinCode = session.JoinCode,
            RequiredParticipants = session.RequiredParticipants,
            CurrentParticipants = current
        };
    }

    // New synchronized game flow
    public async Task<GameStateResponseDTO> GetGameStateAsync(int sessionId, int participantId, CancellationToken ct)
    {
        if (sessionId <= 0)
            throw new ArgumentException("SessionId is required.", nameof(sessionId));
        if (participantId <= 0)
            throw new ArgumentException("ParticipantId is required.", nameof(participantId));

        var session = await _sessions.GetByIdAsync(sessionId, ct)
            ?? throw new NotFoundException("Session not found.");

        var participant = await _participants.GetByIdAsync(participantId, ct)
            ?? throw new NotFoundException("Participant not found.");

        if (participant.SessionId != sessionId || !participant.IsActive)
            throw new ConflictException("Participant does not belong to this session.");

        var currentParticipants = await _participants.CountBySessionIdAsync(sessionId, ct);
        if (currentParticipants < session.RequiredParticipants)
            throw new ConflictException($"Not enough participants. Required {session.RequiredParticipants}, current {currentParticipants}.");

        var now = DateTime.UtcNow;

        await FinalizeExpiredRoundIfNeededAsync(session, now, ct);

        if (!session.IsActive || session.Status == SessionStatus.Completed)
        {
            var winners = await GetCompletedSessionWinnersAsync(sessionId, session.CompletedReason, ct);

            return new GameStateResponseDTO
            {
                Completed = true,
                IsUnanimousMatch = session.CompletedReason == SessionCompletedReason.UnanimousMatch,
                ServerUtcNow = now,
                RoundEndsAtUtc = null,
                RoundNumber = 0,
                CurrentRestaurant = null,
                MyVoteIsLike = null,
                Winners = winners
            };
        }

        var openRound = await _rounds.GetOpenRoundAsync(sessionId, ct);
        if (openRound is null)
        {
            openRound = await CreateNextRoundAsync(session, now, ct);

            if (openRound is null)
            {
                var winners = await GetCompletedSessionWinnersAsync(sessionId, session.CompletedReason, ct);

                return new GameStateResponseDTO
                {
                    Completed = true,
                    IsUnanimousMatch = session.CompletedReason == SessionCompletedReason.UnanimousMatch,
                    ServerUtcNow = now,
                    RoundEndsAtUtc = null,
                    RoundNumber = 0,
                    CurrentRestaurant = null,
                    MyVoteIsLike = null,
                    Winners = winners
                };
            }
        }

        var myVote = openRound.Votes.FirstOrDefault(v => v.ParticipantId == participantId);

        return new GameStateResponseDTO
        {
            Completed = false,
            IsUnanimousMatch = false,
            ServerUtcNow = now,
            RoundEndsAtUtc = openRound.EndsAtUtc,
            RoundNumber = openRound.RoundNumber,
            CurrentRestaurant = MapRestaurant(openRound.Restaurant),
            MyVoteIsLike = myVote?.IsLike,
            Winners = new List<WinnerResponseDTO>()
        };
    }

    public async Task<GameStateResponseDTO> SetVoteAsync(int sessionId, SetVoteRequestDTO request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (sessionId <= 0)
            throw new ArgumentException("SessionId is required.", nameof(sessionId));
        if (request.ParticipantId <= 0)
            throw new ArgumentException("ParticipantId is required.", nameof(request.ParticipantId));

        var session = await _sessions.GetByIdAsync(sessionId, ct)
            ?? throw new NotFoundException("Session not found.");

        var participant = await _participants.GetByIdAsync(request.ParticipantId, ct)
            ?? throw new NotFoundException("Participant not found.");

        if (participant.SessionId != sessionId || !participant.IsActive)
            throw new ConflictException("Participant does not belong to this session.");

        var now = DateTime.UtcNow;

        await FinalizeExpiredRoundIfNeededAsync(session, now, ct);

        if (!session.IsActive || session.Status == SessionStatus.Completed)
            return await GetGameStateAsync(sessionId, request.ParticipantId, ct);

        var round = await _rounds.GetOpenRoundAsync(sessionId, ct);
        if (round is null)
            round = await CreateNextRoundAsync(session, now, ct);

        if (round is null)
            return await GetGameStateAsync(sessionId, request.ParticipantId, ct);

        if (round.EndsAtUtc <= now)
        {
            await FinalizeExpiredRoundIfNeededAsync(session, now, ct);
            return await GetGameStateAsync(sessionId, request.ParticipantId, ct);
        }

        var existingVote = await _rounds.GetVoteAsync(round.Id, request.ParticipantId, ct);

        if (existingVote is null)
        {
            await _rounds.AddVoteAsync(new SessionRoundVote
            {
                SessionRoundId = round.Id,
                ParticipantId = request.ParticipantId,
                IsLike = request.IsLike,
                UpdatedAtUtc = now
            }, ct);
        }
        else
        {
            existingVote.IsLike = request.IsLike;
            existingVote.UpdatedAtUtc = now;
        }

        await _uow.SaveChangesAsync(ct);

        return await GetGameStateAsync(sessionId, request.ParticipantId, ct);
    }

    private async Task<SessionRound?> CreateNextRoundAsync(Session session, DateTime now, CancellationToken ct)
    {
        var selectedCategoryIds = session.UseAllCategories
            ? Array.Empty<int>()
            : session.SessionCategories
                .Select(x => x.CategoryId)
                .Distinct()
                .ToArray();

        var shownRestaurantIds = await _rounds.GetShownRestaurantIdsAsync(session.Id, ct);

        var nextRestaurantId = await _restaurants.GetNextRestaurantIdAsync(
            selectedCityId: session.SelectedCityId,
            useAllCategories: session.UseAllCategories,
            selectedCategoryIds: selectedCategoryIds,
            excludedRestaurantIds: shownRestaurantIds,
            ct: ct);

        if (nextRestaurantId is null)
        {
            session.Status = SessionStatus.Completed;
            session.IsActive = false;
            session.CompletedReason = SessionCompletedReason.DeckExhausted;
            session.CompletedAt = now;

            await _uow.SaveChangesAsync(ct);
            return null;
        }

        var latestRound = await _rounds.GetLatestRoundAsync(session.Id, ct);
        var nextRoundNumber = latestRound?.RoundNumber + 1 ?? 1;

        var newRound = new SessionRound
        {
            SessionId = session.Id,
            RoundNumber = nextRoundNumber,
            RestaurantId = nextRestaurantId.Value,
            StartsAtUtc = now,
            EndsAtUtc = now.AddSeconds(30),
            IsClosed = false
        };

        await _rounds.AddAsync(newRound, ct);
        await _uow.SaveChangesAsync(ct);

        return await _rounds.GetOpenRoundAsync(session.Id, ct);
    }

    private async Task FinalizeExpiredRoundIfNeededAsync(Session session, DateTime now, CancellationToken ct)
    {
        var round = await _rounds.GetOpenRoundAsync(session.Id, ct);
        if (round is null)
            return;

        if (round.EndsAtUtc > now)
            return;

        round.IsClosed = true;

        var activeParticipants = await _participants.GetActiveBySessionIdAsync(session.Id, ct);
        var activeParticipantIds = activeParticipants.Select(p => p.Id).ToHashSet();

        var votes = await _rounds.GetVotesAsync(round.Id, ct);
        var likeParticipantIds = votes
            .Where(v => v.IsLike)
            .Select(v => v.ParticipantId)
            .ToHashSet();

        if (activeParticipantIds.Count > 0 && activeParticipantIds.SetEquals(likeParticipantIds))
        {
            session.Status = SessionStatus.Completed;
            session.IsActive = false;
            session.CompletedReason = SessionCompletedReason.UnanimousMatch;
            session.CompletedAt = now;

            await _actions.SetLikedAsync(session.Id, activeParticipants.First().Id, round.RestaurantId, ct);
            foreach (var participantId in activeParticipantIds.Skip(1))
            {
                await _actions.SetLikedAsync(session.Id, participantId, round.RestaurantId, ct);
            }

            await _uow.SaveChangesAsync(ct);
            return;
        }

        foreach (var vote in votes.Where(v => v.IsLike))
        {
            await _actions.SetLikedAsync(session.Id, vote.ParticipantId, round.RestaurantId, ct);
        }

        foreach (var participantId in activeParticipantIds.Except(votes.Select(v => v.ParticipantId)))
        {
            await _actions.AddSeenIfMissingAsync(session.Id, participantId, round.RestaurantId, ct);
        }

        foreach (var participantId in votes.Where(v => !v.IsLike).Select(v => v.ParticipantId))
        {
            await _actions.AddSeenIfMissingAsync(session.Id, participantId, round.RestaurantId, ct);
        }

        await _uow.SaveChangesAsync(ct);
    }

    private static GameRestaurantDTO MapRestaurant(Restaurant restaurant)
    {
        return new GameRestaurantDTO
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            ImageUrl = restaurant.ImageUrl,
            Categories = restaurant.RestaurantCategories
                .Select(rc => rc.Category.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList()!
        };
    }

    private async Task<List<WinnerResponseDTO>> GetCompletedSessionWinnersAsync(
        int sessionId,
        SessionCompletedReason? completedReason,
        CancellationToken ct)
    {
        if (completedReason is null)
            return new List<WinnerResponseDTO>();

        return completedReason.Value switch
        {
            SessionCompletedReason.UnanimousMatch => await GetMatchWinnersAsync(sessionId, ct),
            SessionCompletedReason.DeckExhausted => await GetTopLikesWinnersAsync(sessionId, ct),
            _ => new List<WinnerResponseDTO>()
        };
    }

    private async Task<List<WinnerResponseDTO>> GetMatchWinnersAsync(int sessionId, CancellationToken ct)
    {
        var participantCount = await _participants.CountActiveAsync(sessionId, ct);
        if (participantCount <= 0) return new List<WinnerResponseDTO>();

        var likeCounts = await _actions.GetLikeCountsByRestaurantAsync(sessionId, ct);

        var matchIds = likeCounts
            .Where(kvp => kvp.Value == participantCount)
            .Select(kvp => kvp.Key)
            .ToList();

        return await BuildWinnersAsync(matchIds, SessionCompletedReason.UnanimousMatch, ct);
    }

    private async Task<List<WinnerResponseDTO>> GetTopLikesWinnersAsync(int sessionId, CancellationToken ct)
    {
        var ids = await ComputeWinnerIdsByTopLikesAsync(sessionId, ct);
        return await BuildWinnersAsync(ids, SessionCompletedReason.DeckExhausted, ct);
    }

    private async Task<List<int>> ComputeWinnerIdsByTopLikesAsync(int sessionId, CancellationToken ct)
    {
        var likeCounts = await _actions.GetLikeCountsByRestaurantAsync(sessionId, ct);
        if (likeCounts.Count == 0) return new List<int>();

        var max = likeCounts.Values.Max();
        return likeCounts.Where(x => x.Value == max).Select(x => x.Key).ToList();
    }

    private async Task<List<WinnerResponseDTO>> BuildWinnersAsync(
        IReadOnlyList<int> winnerIds,
        SessionCompletedReason reason,
        CancellationToken ct)
    {
        if (winnerIds.Count == 0)
            return new List<WinnerResponseDTO>();

        var names = await _restaurants.GetNamesByIdsAsync(winnerIds, ct);

        return winnerIds.Select(id => new WinnerResponseDTO
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