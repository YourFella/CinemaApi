using CinemaApi.Data;
using CinemaApi.Domain;
using CinemaApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/reservations
        // (US-3, US-7) Regular reservation
        [HttpPost]
        public IActionResult ReserveSeats([FromBody] ReserveSeatsRequest request)
        {
            if (request.SeatIds == null || !request.SeatIds.Any())
            {
                return BadRequest("Must provide at least one seat to reserve.");
            }

            using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var showtime = _context.Showtimes
                    .Include(s => s.Movie)
                    .FirstOrDefault(s => s.Id == request.ShowtimeId);

                if (showtime == null) return NotFound($"Showtime with ID {request.ShowtimeId} not found.");

                var requestedSeats = _context.Seats
                    .Where(s => request.SeatIds.Contains(s.Id) && s.AuditoriumId == showtime.AuditoriumId)
                    .ToList();

                if (requestedSeats.Count != request.SeatIds.Count)
                    return BadRequest("One or more invalid seats provided for this showtime's auditorium.");

                var existingReservations = _context.Reservations
                    .Include(r => r.Seats)
                    .Where(r => r.ShowtimeId == request.ShowtimeId)
                    .ToList();

                var currentTime = DateTime.UtcNow;
                var activeReservations = existingReservations.Where(r => !r.IsExpired(currentTime)).ToList();
                var activeSeatIds = activeReservations.SelectMany(r => r.Seats.Select(s => s.Id)).ToList();
                var overlappingSeats = request.SeatIds.Intersect(activeSeatIds).ToList();

                if (overlappingSeats.Any())
                    return BadRequest("One or more requested seats are already reserved or sold.");

                var reservation = new Reservation(request.ShowtimeId, requestedSeats, currentTime);
                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                transaction.Commit();

                return Ok(new
                {
                    ReservationReference = reservation.Id,
                    NumberOfSeats = reservation.Seats.Count,
                    AuditoriumId = showtime.AuditoriumId,
                    MovieTitle = showtime.Movie.Title
                });
            }
            catch (Exception)
            {
                // Conflict in DB (someone else managed to snatch reservation)
                transaction.Rollback();
                return StatusCode(409, "Concurrency conflict: someone else is booking these seats right now. Please try again.");
            }
        }

        // POST: api/reservations/contiguous
        // (US-6, US-7) Contiguous seats reservation
        [HttpPost("contiguous")]
        public IActionResult ReserveContiguousSeats([FromBody] ReserveContiguousSeatsRequest request)
        {
            using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var showtime = _context.Showtimes
                    .Include(s => s.Movie)
                    .FirstOrDefault(s => s.Id == request.ShowtimeId);

                if (showtime == null) return NotFound("Showtime not found.");

                var allSeats = _context.Seats
                    .Where(s => s.AuditoriumId == showtime.AuditoriumId)
                    .OrderBy(s => s.Row).ThenBy(s => s.Number)
                    .ToList();

                var currentTime = DateTime.UtcNow;
                var activeSeatIds = _context.Reservations
                    .Include(r => r.Seats)
                    .Where(r => r.ShowtimeId == request.ShowtimeId)
                    .ToList()
                    .Where(r => !r.IsExpired(currentTime))
                    .SelectMany(r => r.Seats.Select(s => s.Id))
                    .ToHashSet();

                var groupedByRow = allSeats.GroupBy(s => s.Row);
                List<Seat> foundBlock = null; // BETTER NOT TO DO THAT (null)!!!

                foreach (var rowGroup in groupedByRow)
                {
                    var seatsInRow = rowGroup.ToList();
                    var currentBlock = new List<Seat>();

                    foreach (var seat in seatsInRow)
                    {
                        if (!activeSeatIds.Contains(seat.Id))
                        {
                            if (currentBlock.Count == 0 || seat.Number == currentBlock.Last().Number + 1)
                            {
                                currentBlock.Add(seat);
                            }
                            else
                            {
                                currentBlock.Clear();
                                currentBlock.Add(seat);
                            }

                            if (currentBlock.Count == request.SeatCount)
                            {
                                foundBlock = currentBlock;
                                break;
                            }
                        }
                        else
                        {
                            currentBlock.Clear();
                        }
                    }

                    if (foundBlock != null) break;
                }

                if (foundBlock == null)
                {
                    return BadRequest($"Could not find {request.SeatCount} contiguous seats for this showtime.");
                }

                var reservation = new Reservation(request.ShowtimeId, foundBlock, currentTime);
                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                transaction.Commit();

                return Ok(new
                {
                    ReservationReference = reservation.Id,
                    NumberOfSeats = reservation.Seats.Count,
                    ReservedSeats = foundBlock.Select(s => new { s.Row, s.Number }),
                    AuditoriumId = showtime.AuditoriumId,
                    MovieTitle = showtime.Movie.Title
                });
            }
            catch (Exception)
            {
                transaction.Rollback();
                return StatusCode(409, "Concurrency conflict: seats were taken while processing. Please try again.");
            }
        }

        // POST: api/reservations/{id}/confirm
        // (US-4) Reservation confirming
        [HttpPost("{id}/confirm")]
        public IActionResult ConfirmReservation(Guid id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound($"Reservation with ID {id} not found.");

            try
            {
                reservation.Confirm(DateTime.UtcNow);
                _context.SaveChanges();
                return Ok(new { Message = "Reservation successfully confirmed.", ReservationId = id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
