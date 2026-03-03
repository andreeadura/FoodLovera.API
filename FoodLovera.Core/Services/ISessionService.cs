using FoodLovera.Core.Contracts;

namespace FoodLovera.Core.Services;

public interface ISessionService
{
    Task<CreateSessionResponse> CreateAsync(CreateSessionRequest request, CancellationToken ct);
    Task<JoinSessionResponse> JoinAsync(string joinCode, JoinSessionRequest request, CancellationToken ct);
    Task<NextResponse> NextAsync(int sessionId, NextRequest request, CancellationToken ct);
    Task<LikeResponse> LikeAsync(int sessionId, int restaurantId, LikeRequest request, CancellationToken ct);
}