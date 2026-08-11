using CinemaApi.Domain;

namespace CinemaApi.Data
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Is DB exist? If no - create
            context.Database.EnsureCreated();

            // If DB already have Movies than no need to initialize
            if (context.Movies.Any())
            {
                return;
            }

            // 1. Fill with random movies for testing
            var movies = new[]
            {
                new Movie("Inception", "Sci-Fi", 2010),
                new Movie("The Dark Knight", "Action", 2008),
                new Movie("Interstellar", "Sci-Fi", 2014)
            };
            context.Movies.AddRange(movies);

            // 2. One auditorium for testing
            var auditorium = new Auditorium();
            context.Auditoriums.Add(auditorium);

            // 3. 50 seats (5 rows, 10 seats each)
            for (short row = 1; row <= 5; row++)
            {
                for (short number = 1; number <= 10; number++)
                {
                    context.Seats.Add(new Seat(row, number, auditorium.Id));
                }
            }

            // Saving DB
            context.SaveChanges();
        }
    }
}
