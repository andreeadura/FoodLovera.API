using FoodLovera.Models.Models;

namespace FoodLovera.Models.Entities;

public class SessionParticipant:BaseDTO
{
   

    public int SessionId { get; set; }

  

    // Pentru mai târziu (login/JWT)
    public int? UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; }

    public int? CurrentRestaurantId { get; set; }
    public bool IsFinished { get; set; } // default false
}