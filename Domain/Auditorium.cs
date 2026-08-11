namespace CinemaApi.Domain
{
    public class Auditorium
    {
        public Guid Id {  get; set; }
        public List<Seat> Seats { get; set; }
        public Auditorium()
        {
            Id = Guid.NewGuid();
        }
    }
}
