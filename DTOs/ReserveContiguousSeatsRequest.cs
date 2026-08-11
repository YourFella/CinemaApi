using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs
{
    public class ReserveContiguousSeatsRequest
    {
        public Guid ShowtimeId { get; set; }

        [Range(1, 20, ErrorMessage = "Please specify between 1 and 20 seats.")]
        public short SeatCount { get; set; }
    }
}
