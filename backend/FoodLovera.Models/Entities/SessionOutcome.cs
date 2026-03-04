using FoodLovera.Models.Enums;

namespace FoodLovera.Models.Entities;

public sealed class SessionOutcome
{
    public int Id { get; set; }

    public int SessionId { get; set; }
    public Session Session { get; set; } = default!;

    public SessionCompletedReason Reason { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<SessionOutcomeRestaurant> Restaurants { get; set; } = new List<SessionOutcomeRestaurant>();
}