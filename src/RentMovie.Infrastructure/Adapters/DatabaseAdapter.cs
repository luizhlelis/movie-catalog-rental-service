using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Dtos;
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

    public async Task<User> UpdateUserAsync(string username, UpdateUserDto userDto)
    {
        var entry = await _dbContext.Users.FirstAsync(user => user.Username == username);
        entry.Bind(userDto);
        await _dbContext.SaveChangesAsync();
        return entry;
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

    public async Task<Movie?> GetMovieByIdIncludingAsync(Guid id)
    {
        return await _dbContext.Movies.Where(movie => movie.Id == id)
            .Include(movie => movie.Cast)
            .Include(movie => movie.Category)
            .Include(movie => movie.Director)
            .Include(movie => movie.RentCategory)
            .FirstOrDefaultAsync();
    }

    public async Task<Movie?> GetMovieByIdIncludingRentCategoryAsync(Guid id)
    {
        return await _dbContext.Movies
            .Where(movie => movie.Id == id)
            .Include(movie => movie.RentCategory)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Movie>> GetMoviesAsync(int page, int pageSize)
    {
        return await _dbContext.Movies
            .OrderByDescending(post => post.ReleaseYear)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Create a new movie in database. For the nested dependencies (Director, Cast, etc) it works
    /// as an upsert: create if doesn't exist or update the relationship otherwise.
    /// </summary>
    /// <param name="movie">Movie entity with all nested dependencies.</param>
    public async Task<Movie> AddMovieAsync(Movie movie)
    {
        /*  I recognize that this was a bad design decision, the ideal would be to create a CRUD
            for each nested movie dependency */

        var beforeRemoveNested = movie.ShallowCopy();

        var castActorNames = movie.Cast.Select(actor => actor.Name).ToList();

        var getRegisteredActorsTask = _dbContext.Actors
            .Where(actor => castActorNames.Contains(actor.Name)).ToListAsync();
        var registeredDirectorTask = _dbContext.Directors
            .FirstOrDefaultAsync(director => director.Name == movie.Director.Name);
        var registeredCategoryTask = _dbContext.MovieCategories
            .FirstOrDefaultAsync(category => category.Name == movie.Category.Name);
        var registeredRentCategoryTask = _dbContext.RentCategories
            .FirstOrDefaultAsync(category => category.Name == movie.RentCategory.Name);

        await Task.WhenAll(getRegisteredActorsTask, registeredDirectorTask,
            registeredCategoryTask, registeredRentCategoryTask);

        var registeredCast = await getRegisteredActorsTask;
        var registeredDirector = await registeredDirectorTask;
        var registeredCategory = await registeredCategoryTask;
        var registeredRentCategory = await registeredRentCategoryTask;

        var castToCreate = new List<Actor>();
        foreach (var actor in movie.Cast)
        {
            if (!registeredCast.Any(registered => registered.Name == actor.Name))
                castToCreate.Add(actor);
        }

        movie.RemoveNestedProperties();

        movie.Cast = castToCreate;

        var entry = await _dbContext.Movies.AddAsync(movie);

        foreach (var actor in registeredCast)
        {
            actor.Movies ??= new List<Movie>();
            actor.Movies.Add(entry.Entity);
        }

        if (registeredDirector is not null)
        {
            registeredDirector.Movies ??= new List<Movie>();
            registeredDirector.Movies.Add(entry.Entity);
        }
        else
            entry.Entity.Director = beforeRemoveNested.Director;

        if (registeredCategory is not null)
        {
            registeredCategory.Movies ??= new List<Movie>();
            registeredCategory.Movies.Add(entry.Entity);
        }
        else
            entry.Entity.Category = beforeRemoveNested.Category;

        if (registeredRentCategory is not null)
        {
            registeredRentCategory.Movies ??= new List<Movie>();
            registeredRentCategory.Movies.Add(entry.Entity);
        }
        else
            entry.Entity.RentCategory = beforeRemoveNested.RentCategory;

        await _dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task UpdateMovieAsync(Movie movie)
    {
        var entry = _dbContext.Entry(movie);
        entry.State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMoviesAsync(HashSet<Movie> movies)
    {
        foreach (var movie in movies)
            _dbContext.Entry(movie).State = EntityState.Modified;

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

    public async Task<List<Movie>> GetMovieRangeByIdAsync(Guid[] movieIds)
    {
        return await _dbContext.Movies.Where(movie => movieIds.Contains(movie.Id)).ToListAsync();
    }

    public async Task<Order> AddOrderAsync(Order order)
    {
        var entry = await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _dbContext.Orders
            .Where(order => order.Id == id)
            .Include(order => order.Customer)
            .FirstOrDefaultAsync();
    }

    public async Task<Order> DeleteOrderAsync(Guid id)
    {
        var order = await _dbContext.Orders.SingleAsync(order => order.Id == id);
        order.DeleteIt();
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        _dbContext.Entry(order).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        return order;
    }
}
