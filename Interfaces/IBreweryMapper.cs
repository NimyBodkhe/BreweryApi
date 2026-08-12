using BreweryApi.Models;

namespace BreweryApi.Interfaces
{
    public interface IBreweryMapper
    {
        BreweryDto Map(OpenBreweryResponse source);
    }
}
