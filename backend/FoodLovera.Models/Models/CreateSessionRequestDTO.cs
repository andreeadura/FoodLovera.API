namespace FoodLovera.Models.Models;
public sealed class CreateSessionRequestDTO
{
    public string Name { get; init; } = default!;
    public int? SelectedCityId { get; init; }

    public bool UseAllCategories { get; init; } = true;

    public IReadOnlyList<int>? CategoryIds { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }

    public int RequiredParticipants { get; init; } = 2;
}