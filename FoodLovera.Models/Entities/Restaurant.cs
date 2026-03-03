using FoodLovera.Models.Models;

namespace FoodLovera.Models.Entities;

public class Restaurant:BaseDTO
{
    
    public int CityId { get; set; }
    public City City { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<RestaurantCategory> RestaurantCategories { get; set; }
        = new List<RestaurantCategory>();
}