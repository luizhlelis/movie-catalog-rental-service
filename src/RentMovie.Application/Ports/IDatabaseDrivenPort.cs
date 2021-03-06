using System.Linq.Expressions;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Dtos;

namespace RentMovie.Application.Ports;

public interface IDatabaseDrivenPort
{
    Task<User> AddUserAsync(User user);

    Task<User> UpdateUserAsync(string username, UpdateUserDto user);

    Task<User?> GetUserAsync(string username);

    Task<User> DeleteUserAsync(User user);

    Task<Movie?> GetMovieByIdAsync(Guid id);

    Task<Movie?> GetMovieByIdIncludingAsync(Guid id);

    Task<Movie?> GetMovieByIdIncludingRentCategoryAsync(Guid id);

    Task<List<Movie>> GetMoviesAsync(int page, int pageSize);

    Task<Movie> AddMovieAsync(Movie movie);

    Task UpdateMovieAsync(Movie movie);

    Task UpdateMoviesAsync(HashSet<Movie> movies);

    Task<Movie> DeleteMovieAsync(Movie movie);

    Task<List<Movie>> GetMovieByExpressionAsync(Expression<Func<Movie, bool>> expression);

    Task<List<Movie>> GetMovieRangeByIdAsync(Guid[] movieIds);

    Task<Order> AddOrderAsync(Order order);

    Task<Order?> GetOrderByIdAsync(Guid id); // GetOrderIncludingCustomer

    Task<Order> DeleteOrderAsync(Guid id);

    Task<Order> UpdateOrderAsync(Order order);
}
