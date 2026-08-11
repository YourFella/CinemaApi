namespace CinemaApi.Domain
{
    public class Movie
    {
        public Guid Id {  get; set; }
        public string Title { get; private set; }
        public string Category {  get; private set; }
        public int Year {  get; private set; }
        public Movie(string title, string category, int year)
        {
            Id = Guid.NewGuid();
            Title = title;
            Category = category;
            Year = year;
        }

        // For EntityFramework
        protected Movie() { }
    }
}
