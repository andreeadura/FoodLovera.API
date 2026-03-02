namespace FoodLovera.Core.Contracts;

public sealed class CreateSessionRequest
{
    public string Name { get; init; } = default!;
}