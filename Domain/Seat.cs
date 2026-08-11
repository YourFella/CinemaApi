namespace CinemaApi.Domain
{
    public class Seat
    {
        public Guid Id { get; init; }
        public short Row { get; private set; }
        public short Number { get; private set; }

        public Guid AuditoriumId { get; private set; }
        public Auditorium Auditorium { get; private set; }

        public Seat(short row, short number, Guid auditoriumId)
        {
            Id = Guid.NewGuid();
            Row = row;
            Number = number;
            AuditoriumId = auditoriumId;
        }

        protected Seat() { }
    }
}
