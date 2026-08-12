using BreweryApi.Models;

namespace BreweryApi.Interfaces
{
    public interface IBreweryService
    {
        Task<IReadOnlyList<BreweryDto>> GetBreweriesAsync(BreweryQuery query);
    }
}
