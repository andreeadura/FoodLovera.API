namespace FoodLovera.Models.Entities;

public class SessionParticipant
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public string DisplayName { get; set; } = default!;

    // Pentru mai târziu (login/JWT). Rămâne null acum.
    public Guid? UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; }
}