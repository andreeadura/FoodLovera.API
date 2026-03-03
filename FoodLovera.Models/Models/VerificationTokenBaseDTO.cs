#nullable enable
namespace FoodLovera.Models.Entities;

public abstract class VerificationTokenBaseDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CodeHash { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
}