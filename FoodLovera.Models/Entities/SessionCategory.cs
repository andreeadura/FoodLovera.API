namespace FoodLovera.Models.Entities;

public sealed class SessionCategory
{
    public Guid SessionId { get; set; }
    public Session Session { get; set; } = default!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}