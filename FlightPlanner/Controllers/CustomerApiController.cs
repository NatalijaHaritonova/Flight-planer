using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private static readonly object _lock = new object();
        private readonly FlightPlannerDbContext _context;

        public CustomerApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Airports")]
        public IActionResult SearchAirports(string search)
        {
            var airports = FlightStorage.FindAirports(search, _context);
            return Ok(airports);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchFlightRequest request)
        {
            lock(_lock)
            {
                if (FlightStorage.IsValidSearch(request))
                {
                    return Ok(FlightStorage.SearchFlights(request, _context));
                }
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult SearchFlights(int id)
        {
            var flight = _context.Flights
                .Include(f => f.From)
                .Include(f => f.To)
                .SingleOrDefault(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }
    }
}
