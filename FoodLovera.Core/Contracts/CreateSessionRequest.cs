namespace FoodLovera.Core.Contracts;

public sealed class CreateSessionRequest
{
    public string Name { get; init; } = default!;
    public Guid? SelectedCityId { get; init; }

    public bool UseAllCategories { get; init; } = true;

    public IReadOnlyList<Guid>? CategoryIds { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}