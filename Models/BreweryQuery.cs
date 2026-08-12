namespace BreweryApi.Models
{
    public class BreweryQuery
    {
        public string? Search { get; set; }
        public string? City { get; set; }
        public string? SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
