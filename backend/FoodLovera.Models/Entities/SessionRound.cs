namespace FoodLovera.Models.Entities;

public sealed class SessionRound
{
    public int Id { get; set; }

    public int SessionId { get; set; }
    public Session Session { get; set; } = default!;

    public int RoundNumber { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = default!;

    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }

    public bool IsClosed { get; set; }

    public ICollection<SessionRoundVote> Votes { get; set; } = new List<SessionRoundVote>();
}