namespace FoodLovera.Models.Entities;

public class Restaurant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public Guid CityId { get; set; }
    public City City { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<RestaurantCategory> RestaurantCategories { get; set; }
        = new List<RestaurantCategory>();
}