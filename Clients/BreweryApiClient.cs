using BreweryApi.Interfaces;
using BreweryApi.Models;

namespace BreweryApi.Clients
{
    public class BreweryApiClient : IBreweryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BreweryApiClient> _logger;

        public BreweryApiClient(HttpClient httpClient, ILogger<BreweryApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IReadOnlyList<OpenBreweryResponse>> GetBreweriesAsync()
        {
            try
            {
                var breweries = await _httpClient.GetFromJsonAsync<List<OpenBreweryResponse>>("breweries?per_page=200");
                return breweries ?? new List<OpenBreweryResponse>();
            }
            catch (HttpRequestException ex) {
                _logger.LogError(ex, "Error occured while calling open brewery DB API");
                throw;
            }
        }
    }
}
