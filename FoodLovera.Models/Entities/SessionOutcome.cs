using FoodLovera.Models.Enums;

namespace FoodLovera.Models.Entities;

public sealed class SessionOutcome
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }
    public Session Session { get; set; } = default!;

    public SessionCompletedReason Reason { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<SessionOutcomeRestaurant> Restaurants { get; set; } = new List<SessionOutcomeRestaurant>();
}