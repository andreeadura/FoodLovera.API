#nullable enable
namespace FoodLovera.Models.Entities;

public sealed class EmailVerificationToken : VerificationTokenBaseDTO
{
    public User User { get; set; } = default!;
}