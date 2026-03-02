using System;

namespace FoodLovera.Models.Entities;

using FoodLovera.Models.Enums;

public class Session
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string JoinCode { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
    public SessionStatus Status { get; set; }

    public SessionCompletedReason? CompletedReason { get; set; }

    public DateTime? CompletedAt { get; set; }
}


