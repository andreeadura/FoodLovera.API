using FoodLovera.Models.Enums;

namespace FoodLovera.Models.Entities;

public class ParticipantRestaurantAction
{
    public int Id { get; set; }

    public int SessionId { get; set; }
    public int ParticipantId { get; set; }
    public int RestaurantId { get; set; }

    public ParticipantRestaurantActionType ActionType { get; set; }

    public DateTime CreatedAt { get; set; }
}