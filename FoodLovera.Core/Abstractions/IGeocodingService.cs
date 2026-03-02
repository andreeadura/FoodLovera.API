namespace FoodLovera.Core.Abstractions;

public interface IGeocodingService
{
    Task<string?> ReverseGeocodeCityAsync(double latitude, double longitude, CancellationToken ct);
}