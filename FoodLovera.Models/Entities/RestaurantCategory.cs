namespace FoodLovera.Models.Entities;

public class RestaurantCategory
{
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = default!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}