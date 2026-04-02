namespace FoodLovera.Models.Entities;

public sealed class SessionRoundVote
{
    public int Id { get; set; }

    public int SessionRoundId { get; set; }
    public SessionRound SessionRound { get; set; } = default!;

    public int ParticipantId { get; set; }
    public SessionParticipant Participant { get; set; } = default!;

    public bool IsLike { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}