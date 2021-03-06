using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private static readonly object _lock = new object();

        [HttpGet]
        [Authorize]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            lock (_lock)
            {
                var flight = FlightStorage.GetFlight(id);
                if (flight == null)
                    return NotFound();
                return Ok(flight);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("flights/{id}")]
        public IActionResult DeleteFlights(int id)
        {
            lock (_lock)
            {
                FlightStorage.DeleteFlight(id);
                return Ok();
            }
        }

        [HttpPut, Authorize]
        [Route("flights")]
        public IActionResult PutFlights(AddFlightRequest request)
        {
            lock (_lock)
            {
                if (!FlightStorage.IsValidAdd(request))
                {
                    return BadRequest();
                }
                
                if (FlightStorage.Exists(request))
                {
                    return Conflict();
                }
                
                var flight = FlightStorage.AddFlight(request);
                return Created("", flight);
            }
        }
    }
}
