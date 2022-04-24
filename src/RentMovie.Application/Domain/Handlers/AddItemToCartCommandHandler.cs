using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Commands;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Handlers;

public class AddItemToCartCommandHandler : ICartDrivingPort
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;
    private readonly IDistributedCache _cache;

    public AddItemToCartCommandHandler(IDatabaseDrivenPort databaseDrivenPort,
        IDistributedCache cache)
    {
        _databaseDrivenPort = databaseDrivenPort;
        _cache = cache;
    }

    public async Task Handle(AddItemToCartCommand command)
    {
        var getMovieTask =
            _databaseDrivenPort.GetMovieByIdIncludingRentCategoryAsync(command.MovieId);

        var getCartTask = _cache.GetAsync(command.Username);

        await Task.WhenAll(getMovieTask, getCartTask);

        var movie = await getMovieTask;
        var bytes = await getCartTask;
        var cart = new Cart(bytes);

        /*  validator already checks if movie exists and if amount is bigger than zero,
            nullable check here is not necessary */
        cart.AddItem(movie);

        await _cache.SetAsync(command.Username, cart.ToBytes());
    }
}
