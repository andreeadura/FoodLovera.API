namespace FoodLovera.Models.Models;

public sealed class JoinSessionResponseDTO
{
    public int SessionId { get; init; }
    public int ParticipantId { get; init; }
    public string DisplayName { get; init; } = default!;
}