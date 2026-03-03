#nullable enable
using System.ComponentModel.DataAnnotations;

public sealed class ResendVerificationRequestDTO
{
    [Required, EmailAddress]
    public string Email { get; init; } = default!;
}