using CinemaApi.Data;
using CinemaApi.Domain;
using CinemaApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;

        // Dependency Injection:
        // ASP.NET will get DB context by himself.
        public MoviesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/movies
        [HttpPost]
        public IActionResult CreateMovie([FromBody] CreateMovieRequest request)
        {
            // 1. Base validation
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Category))
            {
                return BadRequest("Title and Category are required.");
            }

            // 2. Create domain model (Id generating in constructor)
            var movie = new Movie(request.Title, request.Category, request.Year);

            // 3. Зберігаємо в базу
            _context.Movies.Add(movie);
            _context.SaveChanges();

            // 4. Return created movie with new ID 
            return Ok(new
            {
                id = movie.Id,
                title = movie.Title,
                category = movie.Category,
                year = movie.Year
            });
        }

        // Just GET method to test IF movies were added in DB
        [HttpGet]
        public IActionResult GetMovies()
        {
            var movies = _context.Movies.ToList();
            return Ok(movies);
        }
    }
}
