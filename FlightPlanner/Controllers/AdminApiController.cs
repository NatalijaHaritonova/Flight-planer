using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _lock = new object();

        public AdminApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            lock (_lock)
            {
                var flight = _context.Flights
                    .Include(f => f.From)
                    .Include(f => f.To)
                    .SingleOrDefault(f => f.Id == id);

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
                var flight = _context.Flights
                    .Include(f => f.From)
                    .Include(f => f.To)
                    .SingleOrDefault(f => f.Id == id);

                if (flight == null)
                {
                    return Ok();
                }

                _context.Flights.Remove(flight);
                _context.SaveChanges();

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
                
                if (Exists(request))
                {
                    return Conflict();
                }

                var flight = FlightStorage.ConvertToFlight(request);
                _context.Flights.Add(flight);
                _context.SaveChanges();

                return Created("", flight);
            }
        }

        private bool Exists(AddFlightRequest request)
        {
            return _context.Flights.Any(f =>
            f.Carrier.ToLower().Trim() == request.Carrier.ToLower().Trim() &&
            f.DepartureTime == request.DepartureTime &&
            f.ArrivalTime == request.ArrivalTime &&
            f.From.AirportName.ToLower().Trim() == request.From.AirportName.ToLower().Trim() &&
            f.To.AirportName.ToLower().Trim() == request.To.AirportName.ToLower().Trim());
        }
    }
}
