#nullable enable
using System.ComponentModel.DataAnnotations;

public sealed class ForgotPasswordRequestDTO
{
    [Required, EmailAddress]
    public string Email { get; init; } = default!;
}