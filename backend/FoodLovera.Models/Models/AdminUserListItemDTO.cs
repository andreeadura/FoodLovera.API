namespace FoodLovera.Models.Models;

public sealed class AdminUserListItemDTO
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? Username { get; set; }
    public bool IsEmailVerified { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}