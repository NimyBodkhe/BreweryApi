using BreweryApi.Interfaces;
using BreweryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BreweryApi.Controllers
{
    
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BreweriesController : ControllerBase
    {
        private readonly IBreweryService _breweryService;
        public BreweriesController(IBreweryService breweryService)
        {
            _breweryService = breweryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<BreweryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBreweries([FromQuery] BreweryQuery query)
        {
            var breweries = await _breweryService.GetBreweriesAsync(query);
            return Ok(breweries);
        }

        [HttpGet("suggestions")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSuggestions([FromQuery] string term)
        {
            var suggestions = await _breweryService.GetSuggetionsAsync(term);
            return Ok(suggestions);
        }
    }
}
