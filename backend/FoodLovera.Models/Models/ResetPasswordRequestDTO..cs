#nullable enable
using System.ComponentModel.DataAnnotations;

public sealed class ResetPasswordRequestDTO
{
    [Required, EmailAddress]
    public string Email { get; init; } = default!;

    [Required, MinLength(6), MaxLength(6)]
    public string Code { get; init; } = default!;

    [Required, MinLength(8)]
    public string NewPassword { get; init; } = default!;
}