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

    public async Task<User?> GetUserAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
    }
}
