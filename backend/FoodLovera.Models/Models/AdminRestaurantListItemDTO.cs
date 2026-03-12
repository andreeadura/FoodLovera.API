namespace FoodLovera.Models.Models;

public sealed class AdminRestaurantListItemDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CityId { get; set; }
    public required string CityName { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}