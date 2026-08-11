namespace CinemaApi.Domain
{
    public class Showtime
    {
        public Guid Id { get; init; }

        public Guid MovieId { get; private set; }
        public Movie Movie { get; private set; }

        public Guid AuditoriumId { get; private set; }
        public Auditorium Auditorium { get; private set; }

        public DateTime StartTime { get; private set; }

        public Showtime(Guid movieId, Guid auditoriumId, DateTime startTime)
        {
            Id = Guid.NewGuid();
            MovieId = movieId;
            AuditoriumId = auditoriumId;
            StartTime = startTime;
        }

        protected Showtime() { }
    }
}
