using System.ComponentModel.DataAnnotations;

namespace FoodLovera.Models.Models;

public sealed class SetVoteRequestDTO
{
    [Required]
    public int ParticipantId { get; init; }

    [Required]
    public bool IsLike { get; init; }
}