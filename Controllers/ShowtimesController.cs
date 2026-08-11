using CinemaApi.Data;
using CinemaApi.Domain;
using CinemaApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShowtimesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/showtimes
        [HttpPost]
        public IActionResult CreateShowtime([FromBody] CreateShowtimeRequest request)
        {
            var movieExists = _context.Movies.Any(m => m.Id == request.MovieId);
            if (!movieExists)
            {
                return NotFound($"Movie with ID {request.MovieId} not found.");
            }

            var auditoriumExists = _context.Auditoriums.Any(a => a.Id == request.AuditoriumId);
            if (!auditoriumExists)
            {
                return NotFound($"Auditorium with ID {request.AuditoriumId} not found.");
            }

            var showtime = new Showtime(request.MovieId, request.AuditoriumId, request.StartTime);

            _context.Showtimes.Add(showtime);
            _context.SaveChanges();

            return Ok(new
            {
                id = showtime.Id,
                movieId = showtime.MovieId,
                auditoriumId = showtime.AuditoriumId,
                startTime = showtime.StartTime
            });
        }

        // GET: api/showtimes
        // GET method for testing IF showtimes were added to DB
        [HttpGet]
        public IActionResult GetShowtimes()
        {
            var showtimes = _context.Showtimes.Select(s => new
            {
                s.Id,
                s.MovieId,
                s.AuditoriumId,
                s.StartTime
            }).ToList();

            return Ok(showtimes);
        }
    }
}
