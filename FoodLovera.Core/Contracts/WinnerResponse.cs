using FoodLovera.Models.Enums;

namespace FoodLovera.Core.Contracts;

public sealed class WinnerResponse
{
    public Guid RestaurantId { get; init; }
    public string RestaurantName { get; init; } = default!;
    public SessionCompletedReason Reason { get; init; }
}