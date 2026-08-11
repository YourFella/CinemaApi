using CinemaApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriumsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditoriumsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/auditoriums
        // Helping method for showing Auditoriums IDs
        [HttpGet]
        public IActionResult GetAuditoriums()
        {
            var auditoriums = _context.Auditoriums.Select(a => new { a.Id }).ToList();
            return Ok(auditoriums);
        }

        // GET: api/auditoriums/{id}/seats
        // Helping method for copying Auditoriums IDs
        [HttpGet("{id}/seats")]
        public IActionResult GetSeats(Guid id)
        {
            var seats = _context.Seats
                .Where(s => s.AuditoriumId == id)
                .Select(s => new { s.Id, s.Row, s.Number })
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToList();

            return Ok(seats);
        }
    }
}
