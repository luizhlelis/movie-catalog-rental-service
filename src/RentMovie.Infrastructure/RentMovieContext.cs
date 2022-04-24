using Microsoft.EntityFrameworkCore;
using RentMovie.Application.Domain.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
    }
}
