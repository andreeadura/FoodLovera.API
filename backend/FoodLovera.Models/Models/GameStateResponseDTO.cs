namespace FoodLovera.Models.Models;

public sealed class GameStateResponseDTO
{
    public bool Completed { get; init; }
    public bool IsUnanimousMatch { get; init; }

    public DateTime ServerUtcNow { get; init; }
    public DateTime? RoundEndsAtUtc { get; init; }

    public int RoundNumber { get; init; }

    public GameRestaurantDTO? CurrentRestaurant { get; init; }

    public bool? MyVoteIsLike { get; init; }

    public List<WinnerResponseDTO> Winners { get; init; } = new();
}