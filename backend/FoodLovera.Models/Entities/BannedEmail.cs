#nullable enable
namespace FoodLovera.Models.Entities;

public sealed class BannedEmail
{
    public int Id { get; set; }

    public string EmailNormalized { get; set; } = null!;

    public DateTime BannedAtUtc { get; set; } = DateTime.UtcNow;

    public int? BannedByUserId { get; set; } // admin (optional)
    public string? Reason { get; set; }
}