using System.Text.Json;

namespace User.Api.Services;

public class GeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeolocationService> _logger;

    public GeolocationService(HttpClient httpClient, ILogger<GeolocationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GeolocationData?> GetLocationFromIpAsync(string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1" || ipAddress.StartsWith("127."))
        {
            // Return default location for localhost
            return new GeolocationData
            {
                City = "Unknown",
                Country = "Unknown",
                Latitude = 0,
                Longitude = 0
            };
        }

        try
        {
            // Using ip-api.com free tier (no API key required, 45 req/min limit)
            var response = await _httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get geolocation for IP {IpAddress}: {StatusCode}", 
                    ipAddress, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IpApiResponse>(content);

            if (result?.Status == "success")
            {
                return new GeolocationData
                {
                    City = result.City,
                    Country = result.Country,
                    Latitude = result.Lat,
                    Longitude = result.Lon
                };
            }

            _logger.LogWarning("Geolocation API returned failure for IP {IpAddress}", ipAddress);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting geolocation for IP {IpAddress}", ipAddress);
            return null;
        }
    }

    private class IpApiResponse
    {
        public string? Status { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}

public class GeolocationData
{
    public required string City { get; set; }
    public required string Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
