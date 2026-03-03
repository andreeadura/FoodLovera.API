namespace FoodLovera.Models.Entities;

public sealed class SessionCategory
{
    public int SessionId { get; set; }
    public Session Session { get; set; } = default!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}