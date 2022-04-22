using Microsoft.EntityFrameworkCore;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Infrastructure.Adapters;

public class RentMovieContext : DbContext
{
    public RentMovieContext(DbContextOptions<RentMovieContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
    }
}
