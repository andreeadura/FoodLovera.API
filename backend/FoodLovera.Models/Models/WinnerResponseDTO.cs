using FoodLovera.Models.Enums;

namespace FoodLovera.Models.Models;

public sealed class WinnerResponseDTO
{
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = default!;
    public SessionCompletedReason Reason { get; init; }
}