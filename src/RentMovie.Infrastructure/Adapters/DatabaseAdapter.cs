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

    public async Task DeleteUserAsync(User user)
    {
        var entry = _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public Task<List<Movie>> GetMovieByExpressionAsync(Expression<Func<Movie, bool>> expression)
    {
        throw new NotImplementedException();
    }
}
