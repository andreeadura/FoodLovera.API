namespace FoodLovera.Core.Contracts;

public sealed class LikeResponse
{
    public bool Completed { get; init; }
    public Guid? CurrentRestaurantId { get; init; }
    public List<WinnerResponse> Winners { get; init; } = new();

}