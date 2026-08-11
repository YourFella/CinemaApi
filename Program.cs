
using CinemaApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CinemaApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Swagger settings
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Connecting SQLite DB
            builder.Services.AddDbContext<AppDbContext>(options 
                => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            //Turning Swagger OFF
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Initialize test DB
            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                DbInitializer.Initialize(context);
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
