using BreweryApi.Interfaces;
using BreweryApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BreweryApi.Services
{
    public class BreweryService : IBreweryService
    {
        private const string BreweryCacheKey = "open-brewery-cache";

        private readonly IBreweryApiClient _breweryApiClient;
        private readonly IBreweryMapper _breweryMapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<BreweryService> _logger;

        public BreweryService(IBreweryApiClient breweryApiClient, IBreweryMapper breweryMapper, IMemoryCache memoryCache, ILogger<BreweryService> logger)
        {
            _breweryApiClient = breweryApiClient;
            _breweryMapper = breweryMapper;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<IReadOnlyList<BreweryDto>> GetBreweriesAsync(BreweryQuery query)
        {
            var breweries = await GetCachedBreweriesAsync();
            var result = breweries.AsEnumerable();

            result = ApplySearch(result, query.Search);
            result = ApplyCityFilter(result, query.City);
            result = ApplySorting(result, query);
            return result.ToList();
        }

        private async Task<IReadOnlyList<BreweryDto>> GetCachedBreweriesAsync()
        {
            if (_memoryCache.TryGetValue(BreweryCacheKey, out IReadOnlyList<BreweryDto> cachedBreweries) && cachedBreweries is not null)
            {
                _logger.LogInformation("Breweries return from in memory cache");
                return cachedBreweries;
            }
            _logger.LogInformation("Cache has expired, calling open brewery DB api");
            var sourceBreweries = await _breweryApiClient.GetBreweriesAsync();
            var mappedBreweries = sourceBreweries.Where(x => !string.Equals(x.BreweryType, "closed", StringComparison.OrdinalIgnoreCase))
                .Select(_breweryMapper.Map)
                .ToList();

            var cacheOption = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            _memoryCache.Set(BreweryCacheKey, mappedBreweries, cacheOption);
            return mappedBreweries;
        }

        private static IEnumerable<BreweryDto> ApplySearch(IEnumerable<BreweryDto> breweries, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return breweries;
            }

            return breweries.Where(x =>
            x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            x.City.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            x.Phone.Contains(search, StringComparison.OrdinalIgnoreCase)
            );
        }

        private static IEnumerable<BreweryDto> ApplyCityFilter(IEnumerable<BreweryDto> breweries, string? city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return breweries;
            }
            return breweries.Where(x => x.City.Equals(city, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<BreweryDto> ApplySorting(IEnumerable<BreweryDto> breweries, BreweryQuery query)
        {
            var sortBy = query.SortBy?.Trim().ToLowerInvariant();
            var sortOrder = query.SortOrder?.Trim().ToLowerInvariant();

            var isDesending = sortOrder == "desc";

            return sortBy switch
            {
                "name" => isDesending ? breweries.OrderByDescending(x => x.Name) : breweries.OrderBy(x => x.Name),
                "city" => isDesending ? breweries.OrderByDescending(x => x.City) : breweries.OrderBy(x => x.City),
                "distance" => SortByDistance(breweries, query, isDesending),
                null or "" => breweries.OrderBy(x => x.Name),
                _ => throw new ArgumentException("Invalid sortBy value, allowed values are name, city, distance")
            };
        }


        private static IEnumerable<BreweryDto> SortByDistance(IEnumerable<BreweryDto> breweries, BreweryQuery query, bool isDesending)
        {
            if (query.Latitude is null || query.Longitude is null)
            {
                throw new ArgumentException("Latitiude and Longitude are required for distance sorting");
            }
            var breweriesWithDistance = breweries.Select(x =>
            {
                x.DistanceInMiles = CalculateDistanceinMiles(query.Latitude.Value, query.Longitude.Value, x.Latitude, x.Longitude);
                return x;
            });
            return isDesending ? breweriesWithDistance.OrderByDescending(x => x.DistanceInMiles
            ?? double.MaxValue) : breweriesWithDistance.OrderBy(x => x.DistanceInMiles ?? double.MaxValue);
        }

        private static double? CalculateDistanceinMiles(double sourceLatitude, double sourceLongitude, double? destinationLatitude, double? destinationLongitude)
        {
            if (destinationLatitude is null || destinationLongitude is null)
            {
                return null;
            }
            const double earthRadiusInMiles = 3958.8;

            var dLat = ToRadians(destinationLatitude.Value - sourceLatitude);
            var DLon = ToRadians(destinationLongitude.Value - sourceLongitude);

            var lat1 = ToRadians(sourceLatitude);
            var lat2 = ToRadians(destinationLatitude.Value);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(DLon / 2) * Math.Sin(DLon / 2);

            var c = 2 * Math.Asin(Math.Sqrt(a));

            return Math.Round(earthRadiusInMiles * c, 2);
        }

        private static double ToRadians(double degree)
        {
            return degree * Math.PI / 180;
        }

        public async Task<IReadOnlyList<string>> GetSuggetionsAsync(string term)
        {
            if(string.IsNullOrWhiteSpace(term))
            {
                return Array.Empty<string>();
            }

            var breweries = await GetCachedBreweriesAsync();
            return breweries
                .Where(x => x.Name.ToLower().Contains(term.ToLower(), StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Name)
                .Distinct()
                .Take(10)
                .ToList();
        }
    }
}
