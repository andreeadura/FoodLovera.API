namespace FoodLovera.Core.Contracts;

public sealed class JoinSessionResponse
{
    public Guid SessionId { get; init; }
    public Guid ParticipantId { get; init; }
    public string DisplayName { get; init; } = default!;
}