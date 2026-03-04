#nullable enable
using System.ComponentModel.DataAnnotations;

public sealed class ChangePasswordRequestDTO
{
    [Required]
    public string CurrentPassword { get; init; } = default!;

    [Required, MinLength(8)]
    public string NewPassword { get; init; } = default!;
}