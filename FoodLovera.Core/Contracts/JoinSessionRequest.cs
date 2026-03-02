namespace FoodLovera.Core.Contracts;

public sealed class JoinSessionRequest
{
    public string DisplayName { get; init; } = default!;
}