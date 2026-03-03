namespace FoodLovera.Core.Contracts;

public sealed class CreateSessionRequest
{
    public string Name { get; init; } = default!;
    public int? SelectedCityId { get; init; }

    public bool UseAllCategories { get; init; } = true;

    public IReadOnlyList<int>? CategoryIds { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}