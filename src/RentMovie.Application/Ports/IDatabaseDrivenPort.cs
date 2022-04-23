using System.Linq.Expressions;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Ports;

public interface IDatabaseDrivenPort
{
    Task<User> AddUserAsync(User user);

    Task<User?> GetUserAsync(string username);

    Task DeleteUserAsync(User user);

    Task<List<Movie>> GetMovieByExpressionAsync(Expression<Func<Movie, bool>> expression);
}
