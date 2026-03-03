namespace FoodLovera.Core.Contracts;

public interface IGeocodingService
{
    Task<string?> ReverseGeocodeCityAsync(double latitude, double longitude, CancellationToken ct);
}