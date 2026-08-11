namespace CinemaApi.DTOs
{
    public class ReserveSeatsRequest
    {
        public Guid ShowtimeId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
    }
}
