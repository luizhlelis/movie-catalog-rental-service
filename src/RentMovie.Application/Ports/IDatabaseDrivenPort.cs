using System.Linq.Expressions;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Ports;

public interface IDatabaseDrivenPort
{
    Task<User> AddUserAsync(User user);

    Task<User?> GetUserAsync(string username);

    Task<User> DeleteUserAsync(User user);

    Task<Movie?> GetMovieByIdAsync(Guid id);

    Task<Movie?> GetMovieByIdIncludingRentCategoryAsync(Guid id);

    Task<List<Movie>> GetMoviesAsync(int page, int pageSize);

    Task<Movie> AddMovieAsync(Movie movie);

    Task UpdateMovieAsync();

    Task<Movie> DeleteMovieAsync(Movie movie);

    Task<List<Movie>> GetMovieByExpressionAsync(Expression<Func<Movie, bool>> expression);

    Task<List<Movie>> GetMovieRangeByIdAsync(Guid[] movieIds);

    Task<Order> AddOrderAsync(Order order);

    Task<Order?> GetOrderByIdAsync(Guid id); // GetOrderIncludingCustomer

    Task<Order> DeleteOrderAsync(Guid id);

    Task UpdateEntriesAsync();
}
