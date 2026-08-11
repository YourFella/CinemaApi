using CinemaApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace CinemaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tables in Database
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // n-to-n relations between Reservation and Seat (better not to do that)
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Seats)
                .WithMany();

            // Movie title is mandatory
            modelBuilder.Entity<Movie>()
                .Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
