using FlightPlanner.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        [HttpGet]
        [Route("Airports")]
        public IActionResult SearchAirports(string search)
        {
            var airports = FlightStorage.FindAirports(search);
            return Ok(airports);
        }
    }
}
