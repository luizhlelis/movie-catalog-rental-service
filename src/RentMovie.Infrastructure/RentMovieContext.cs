using Microsoft.EntityFrameworkCore;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;

namespace RentMovie.Infrastructure;

public class RentMovieContext : DbContext
{
    public RentMovieContext(DbContextOptions<RentMovieContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Movie> Movies { get; set; }

    public DbSet<RentCategory> RentCategories { get; set; }

    public DbSet<Actor> Actors { get; set; }

    public DbSet<Director> Directors { get; set; }

    public DbSet<MovieCategory> MovieCategories { get; set; }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        // users
        var users = new List<User>
        {
            new("admin-user", "StrongPassword@123", "980395900", "5036 Tierra Locks Suite 158",
                "Admin User", Role.Admin),
            new("customer-user", "StrongPassword@123", "948019535", "570 Hackett Bridge",
                "Customer User")
        };
        modelBuilder.Entity<User>().HasData(users);

        // movies
        var movie1 = new Movie("The Batman",
            "When the Riddler, a sadistic serial killer, begins murdering key political figures in Gotham, Batman is forced to investigate the city's hidden corruption and question his family's involvement.",
            2022, 10);

        modelBuilder.Entity<Movie>().HasData(movie1);
        modelBuilder.Entity<RentCategory>().HasData(new
        {
            Name = "Release",
            Price = 5.0,
            Movie = movie1
        });
        modelBuilder.Entity<MovieCategory>().HasData(new { Movie = movie1, Name = "Action" });
        modelBuilder.Entity<Director>().HasData(new { Movie = movie1, Name = "Matt Reeves" });
        modelBuilder.Entity<Actor>().HasData(new List<object>
        {
            new {Movie = movie1, Name = "Robert Pattinson"},
            new {Movie = movie1, Name = "Zoë Kravitz"},
            new {Movie = movie1, Name = "Colin Farrell"},
            new {Movie = movie1, Name = "Paul Dano"}
        });
    }
}
