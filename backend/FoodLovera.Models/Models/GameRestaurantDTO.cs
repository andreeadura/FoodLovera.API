namespace FoodLovera.Models.Models;

public sealed class GameRestaurantDTO
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string ImageUrl { get; init; } = default!;
    public List<string> Categories { get; init; } = new();
}