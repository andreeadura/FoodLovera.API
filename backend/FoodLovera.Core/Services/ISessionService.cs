using FoodLovera.Models.Models;

namespace FoodLovera.Core.Services;

public interface ISessionService
{
    Task<CreateSessionResponseDTO> CreateAsync(CreateSessionRequestDTO request, CancellationToken ct);
    Task<JoinSessionResponseDTO> JoinAsync(string joinCode, JoinSessionRequestDTO request, CancellationToken ct);
    Task<NextResponseDTO> NextAsync(int sessionId, NextRequestDTO request, CancellationToken ct);
    Task<LikeResponseDTO> LikeAsync(int sessionId, int restaurantId, LikeRequestDTO request, CancellationToken ct);
    Task<SessionStatusResponseDTO> GetStatusAsync(int sessionId, CancellationToken ct);

    Task<GameStateResponseDTO> GetGameStateAsync(int sessionId, int participantId, CancellationToken ct);
    Task<GameStateResponseDTO> SetVoteAsync(int sessionId, SetVoteRequestDTO request, CancellationToken ct);
}