namespace FoodLovera.Models.Models;

public sealed class CreateSessionResponseDTO
{
    public int SessionId { get; init; }
    public string JoinCode { get; init; } = default!;
    public string Name { get; init; } = default!;
}