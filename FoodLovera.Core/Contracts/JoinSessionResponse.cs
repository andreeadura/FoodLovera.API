namespace FoodLovera.Core.Contracts;

public sealed class JoinSessionResponse
{
    public int SessionId { get; init; }
    public int ParticipantId { get; init; }
    public string DisplayName { get; init; } = default!;
}