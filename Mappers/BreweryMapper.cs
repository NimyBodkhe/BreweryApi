using BreweryApi.Interfaces;
using BreweryApi.Models;

namespace BreweryApi.Mappers
{
    public class BreweryMapper : IBreweryMapper
    {
        public BreweryDto Map(OpenBreweryResponse source)
        {
            return new BreweryDto
            {
                Name = source.Name ?? string.Empty,
                City = source.City ?? string.Empty,
                Phone = source.Phone ?? string.Empty,
                Latitude = Convert.ToDouble(source.Latitude),
                Longitude = Convert.ToDouble(source.Longitude)
            };
        }
    }
}
