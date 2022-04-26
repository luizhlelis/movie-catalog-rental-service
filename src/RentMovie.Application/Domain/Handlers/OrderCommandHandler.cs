using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using RentMovie.Application.Commands;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Handlers;

public class OrderCommandHandler : IOrderDrivingPort
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;
    private readonly IConfiguration _configuration;
    private readonly IPaymentDrivenPort _paymentDrivenPort;
    private readonly IDistributedCache _cache;

    public OrderCommandHandler(IDatabaseDrivenPort databaseDrivenPort,
        IConfiguration configuration,
        IPaymentDrivenPort paymentDrivenPort,
        IDistributedCache cache)
    {
        _databaseDrivenPort = databaseDrivenPort;
        _configuration = configuration;
        _paymentDrivenPort = paymentDrivenPort;
        _cache = cache;
    }

    public async Task<Order?> Handle(CreateOrderCommand command)
    {
        var cartContent = await _cache.GetAsync(command.Username);
        var cart = new Cart(cartContent);

        if (cart.IsEmpty()) return null;

        var originZipCode = _configuration["Shipping:OriginZipCode"];
        var user = await _databaseDrivenPort.GetUserAsync(command.Username);

        var order = new Order(user, cart.TotalPrice, command.PaymentMethod);
        order.LoadShippingPrice(originZipCode);
        order.LoadTotalPrice();

        var entity = await _databaseDrivenPort.AddOrderAsync(order);

        return entity;
    }

    public async Task<Order?> Handle(FinishOrderCommand command)
    {
        var cartContent = await _cache.GetAsync(command.Username);
        var cart = new Cart(cartContent);
        var movieIds = cart.Items.Select(item => item.Key).ToArray();

        var getOrderTask = _databaseDrivenPort.GetOrderByIdAsync(command.OrderId);
        var getMoviesTask = _databaseDrivenPort.GetMovieRangeByIdAsync(movieIds);

        await Task.WhenAll(getOrderTask, getMoviesTask);

        var order = await getOrderTask;
        var movies = await getMoviesTask;

        await _paymentDrivenPort.ChargeUser(order.TotalPrice, order.PaymentMethod,
            command.Username);

        var moviesToUpdate = new HashSet<Movie>();

        foreach (var movie in movies)
        {
            var itemInCart = cart.Items[movie.Id];
            movie.RemoveAmountFromStock(itemInCart.Amount);

            if (movie.AmountAvailable < 0)
                return null;

            moviesToUpdate.Add(movie);
        }

        order.FinalizeIt();

        var removeFromCacheTask = _cache.RemoveAsync(command.Username);
        var updateOrderTask = _databaseDrivenPort.UpdateOrderAsync(order);
        var updateMoviesTask = _databaseDrivenPort.UpdateMoviesAsync(moviesToUpdate);

        await Task.WhenAll(removeFromCacheTask, updateOrderTask, updateMoviesTask);

        return await updateOrderTask;
    }
}
