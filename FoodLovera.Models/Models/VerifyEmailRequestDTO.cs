#nullable enable
using System.ComponentModel.DataAnnotations;

public sealed class VerifyEmailRequestDTO
{
    [Required, EmailAddress]
    public string Email { get; init; } = default!;

    [Required]
    [MinLength(6), MaxLength(6)]
    public string Code { get; init; } = default!;
}