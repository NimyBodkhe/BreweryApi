using BreweryApi.Models;

namespace BreweryApi.Interfaces
{
    public interface IBreweryApiClient
    {
        Task<IReadOnlyList<OpenBreweryResponse>> GetBreweriesAsync();
    }
}
