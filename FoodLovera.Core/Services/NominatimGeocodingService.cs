using System.Net.Http.Json;
using FoodLovera.Core.Abstractions;

namespace FoodLovera.Infrastructure.Services;

public sealed class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _http;

    public NominatimGeocodingService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> ReverseGeocodeCityAsync(double latitude, double longitude, CancellationToken ct)
    {
        try
        {
            var url = $"reverse?format=jsonv2&lat={latitude}&lon={longitude}";
            var resp = await _http.GetFromJsonAsync<NominatimResponse>(url, ct);

            var a = resp?.Address;
            return a?.City ?? a?.Town ?? a?.Municipality ?? a?.Village;
        }
        catch
        {
            return null;
        }
    }

    private sealed class NominatimResponse
    {
        public NominatimAddress? Address { get; set; }

        public sealed class NominatimAddress
        {
            public string? City { get; set; }
            public string? Town { get; set; }
            public string? Village { get; set; }
            public string? Municipality { get; set; }
        }
    }
}