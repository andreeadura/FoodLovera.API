using System;

namespace FoodLovera.Models.Entities;

using FoodLovera.Models.Enums;
using FoodLovera.Models.Models;

public class Session:BaseDTO
{
    
    public string JoinCode { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
    public SessionStatus Status { get; set; }

    public SessionCompletedReason? CompletedReason { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid SelectedCityId { get; set; }
    public bool UseAllCategories { get; set; } = true;

    public ICollection<SessionCategory> SessionCategories { get; set; }
        = new List<SessionCategory>();
}





