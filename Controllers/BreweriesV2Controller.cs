using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BreweryApi.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BreweriesV2Controller : ControllerBase
    {
        //add any new dependencies or services for version 2.0 here
        public BreweriesV2Controller()
        {
            //initialize any new dependencies or services for version 2.0 here
        }
        [HttpGet]
        public async Task<IActionResult> GetBreweries()
        {
            // Implementation for version 2.0
            return Ok("This is version 2.0");
        }
    }
}
