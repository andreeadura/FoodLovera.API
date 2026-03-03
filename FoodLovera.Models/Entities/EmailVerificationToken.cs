#nullable enable
namespace FoodLovera.Models.Entities;

public sealed class EmailVerificationToken
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public string CodeHash { get; set; } = default!;

    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int AttemptCount { get; set; } = 0;
    public DateTime? LastAttemptAtUtc { get; set; }
}