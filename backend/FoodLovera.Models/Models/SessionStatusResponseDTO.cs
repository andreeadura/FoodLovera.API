namespace FoodLovera.Models.Models;

public sealed class SessionStatusResponseDTO
{
    public int SessionId { get; init; }
    public string JoinCode { get; init; } = default!;
    public int RequiredParticipants { get; init; }
    public int CurrentParticipants { get; init; }
}