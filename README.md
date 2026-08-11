# Cinema Showtimes API

A REST API for managing cinema showtimes, movie reservations, and seat bookings. Built with ASP.NET Core Web API and Entity Framework Core (SQLite).

This project was created as a take-home assessment and implements all core and optional user stories (US-1 to US-7), including time-expiring reservations, contiguous seat booking, and concurrency safety.

# Features (User Stories Completed)

* US-1 (Create Movie): Add a movie to the catalog (Id, title, category, year). Database is seeded with sample movies on startup.

* US-2 (Create Showtime): Schedule a movie in a specific auditorium at a given time.

* US-3 (Reserve Seats): Reserve specific seats for a showtime. Prevents double-booking. Reservations automatically expire after 10 minutes.

* US-4 (Confirm Reservation): Buy/confirm an active reservation using its unique reference ID. Cannot confirm expired reservations.

* US-5 (Persistence): Data is saved using Entity Framework Core with an SQLite database (cinema.db), ensuring data survives application restarts.

* US-6 (Contiguous Seats - Optional): Added a /api/reservations/contiguous endpoint to automatically find and book adjacent seats in the same row.

* US-7 (Concurrency Safety - Optional): Implemented database transactions with Serializable isolation level in EF Core to guarantee that no seat is double-booked, even under simultaneous high-load requests.

# Tech Stack

* C# / .NET 10.0

* ASP.NET Core Web API

* Entity Framework Core

* SQLite (For Database)

* Swagger (for Testing)

# How to Run the Project

1. Prerequisites: Ensure you have the .NET SDK installed.

2. Clone the repository:

> git clone URL

> cd CinemaApi

3. Run the application:

* Via Visual Studio: Open the .sln file and press F5 (or the green Play button) to run with IIS Express / Kestrel.

* Via CLI:

> dotnet run

4. Database Initialization: You do not need to run any database migrations manually. On the first startup, the application will automatically create the cinema.db SQLite file and seed it with initial data (3 movies, 1 auditorium, and 50 seats).

5. Open Swagger UI:
Navigate to https://localhost:<port>/swagger (or check your console output for the exact URL) in your browser to interact with the API endpoints.

# API Endpoints Overview

* GET /api/movies - Get seeded movies.

* POST /api/movies - Create a new movie.

* GET /api/showtimes - Get all showtimes.

* POST /api/showtimes - Create a new showtime.

* POST /api/reservations - Reserve specific seats by IDs.

* POST /api/reservations/contiguous - Reserve N contiguous seats automatically.

* POST /api/reservations/{id}/confirm - Confirm (buy) an active reservation.

* GET /api/auditoriums & GET /api/auditoriums/{id}/seats - Helper endpoints to retrieve IDs for testing.

# Design Decisions

* Rich Domain Model: The Reservation entity encapsulates its own business logic (IsExpired(), Confirm()). This prevents state corruption from outside the class.

* Concurrency Handling: To solve US-7, the booking endpoints wrap their read-check-write logic in a Serializable transaction (*_context.Database.BeginTransaction(IsolationLevel.Serializable)*). If two requests attempt to book the same seats at the exact same millisecond, the database engine will reject the conflicting transaction, and the API will return an HTTP 409 Conflict.

# What to change!!!

1. DB is commited in repo. Better to add in .gitignore
2. Migration problem. Not realy for a demo, but on real project better to use *donet ef migrations*.
3. Obviously: no auth. *app.UseAuthorization()* isinvoked but there is nothing behind it right now.
4. Visible problem with try/cathes (specifically in *ReservationsController*). This could throw unexpected errors and cause problems down the line.
5. *ReserveSeats* and *ReserveContiguousSeats* almost identical and could be joined into interface *(maybe)*
6. Normal form in DB. *AppDbContext* from *OnModelCreating* have n-to-n between Reservation and Seat without implicit join-table it could be problem in future "is X seat free" requests. *(but that's related to DB architecture)*