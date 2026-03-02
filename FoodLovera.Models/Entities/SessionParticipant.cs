using FoodLovera.Models.Models;

namespace FoodLovera.Models.Entities;

public class SessionParticipant:BaseDTO
{
   

    public Guid SessionId { get; set; }

  

    // Pentru mai târziu (login/JWT)
    public Guid? UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; }

    public Guid? CurrentRestaurantId { get; set; }
    public bool IsFinished { get; set; } // default false
}