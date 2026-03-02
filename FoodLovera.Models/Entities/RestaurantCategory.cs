namespace FoodLovera.Models.Entities;

public class RestaurantCategory
{
    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = default!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}