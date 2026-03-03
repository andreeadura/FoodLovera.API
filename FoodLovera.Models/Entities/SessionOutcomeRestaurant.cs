namespace FoodLovera.Models.Entities;

public sealed class SessionOutcomeRestaurant
{
    public int OutcomeId { get; set; }
    public SessionOutcome Outcome { get; set; } = default!;

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = default!;

    public int LikeCount { get; set; }
}