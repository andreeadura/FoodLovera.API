using FoodLovera.Models.Enums;

namespace FoodLovera.Models.Entities;

public class ParticipantRestaurantAction
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid RestaurantId { get; set; }

    public ParticipantRestaurantActionType ActionType { get; set; }

    public DateTime CreatedAt { get; set; }
}