using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Ports;

namespace RentMovie.Infrastructure.Adapters;

// Database Adapter: implements the driven port
public class DatabaseAdapter : IDatabaseDrivenPort
{
    private readonly RentMovieContext _dbContext;

    public DatabaseAdapter(RentMovieContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> AddUserAsync(User user)
    {
        var entry = await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<User?> GetUserAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<User> DeleteUserAsync(User user)
    {
        var entry = _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Movie?> GetMovieByIdAsync(Guid id)
    {
        return await _dbContext.Movies.FirstOrDefaultAsync(movie => movie.Id == id);
    }

    public async Task<Movie> AddMovieAsync(Movie movie)
    {
        var entry = await _dbContext.Movies.AddAsync(movie);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task UpdateMovieAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Movie> DeleteMovieAsync(Movie movie)
    {
        var entry = _dbContext.Movies.Remove(movie);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<List<Movie>> GetMovieByExpressionAsync(
        Expression<Func<Movie, bool>> expression)
    {
        return await _dbContext.Movies.Where(expression).ToListAsync();
    }
}
