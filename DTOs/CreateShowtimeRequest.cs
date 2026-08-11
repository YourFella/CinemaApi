namespace CinemaApi.DTOs
{
    public class CreateShowtimeRequest
    {
        public Guid MovieId { get; set; }
        public Guid AuditoriumId { get; set; }
        public DateTime StartTime { get; set; }
    }
}
