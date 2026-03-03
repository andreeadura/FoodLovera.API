namespace FoodLovera.Core.Contracts;

public sealed class CreateSessionResponse
{
    public int SessionId { get; init; }
    public string JoinCode { get; init; } = default!;
    public string Name { get; init; } = default!;
}