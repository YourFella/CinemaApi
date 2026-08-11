namespace CinemaApi.DTOs
{
    public class CreateMovieRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
