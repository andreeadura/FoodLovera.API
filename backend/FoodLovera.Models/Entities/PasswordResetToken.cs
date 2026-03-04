#nullable enable
namespace FoodLovera.Models.Entities;

public sealed class PasswordResetToken : VerificationTokenBaseDTO
{
    public User User { get; set; } = default!;
}