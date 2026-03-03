namespace FoodLovera.Models.Models;

public sealed class CreateRestaurantRequestDTO
{
    public required string Name { get; set; }
    public int CityId { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}