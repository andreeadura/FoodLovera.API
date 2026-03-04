

namespace FoodLovera.Models.Models;

public sealed class LikeResponseDTO
{
    public bool Completed { get; init; }
    public int? CurrentRestaurantId { get; init; }
    public List<WinnerResponseDTO> Winners { get; init; } = new();

}