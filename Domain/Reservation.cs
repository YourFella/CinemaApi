namespace CinemaApi.Domain
{
    public class Reservation
    {
        public Guid Id { get; init; } // This is Reference ID (For US-3)

        public Guid ShowtimeId { get; private set; }
        public Showtime Showtime { get; private set; }

        public List<Seat> Seats { get; private set; } = new();

        public DateTime CreatedAt { get; private set; }
        public bool IsConfirmed { get; private set; } // Status: reserved or bought

        public Reservation(Guid showtimeId, List<Seat> seats, DateTime createdAt)
        {
            if (seats == null || !seats.Any())
                throw new ArgumentException("Reservation must contain at least one seat.");

            Id = Guid.NewGuid();
            ShowtimeId = showtimeId;
            Seats = seats;
            CreatedAt = createdAt;
            IsConfirmed = false;
        }

        protected Reservation() { }

        // Business logic from US-3 and US-4
        public bool IsExpired(DateTime currentTime)
        {
            // Reservation is not expired if it is confirmed
            if (IsConfirmed) return false;

            return currentTime > CreatedAt.AddMinutes(10);
        }

        public void Confirm(DateTime currentTime)
        {
            if (IsConfirmed)
                throw new InvalidOperationException("Reservation is already confirmed.");

            if (IsExpired(currentTime))
                throw new InvalidOperationException("Cannot confirm an expired reservation.");

            IsConfirmed = true;
        }
    }
}
