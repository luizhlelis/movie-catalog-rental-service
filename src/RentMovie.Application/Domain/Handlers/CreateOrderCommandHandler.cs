using Microsoft.Extensions.Configuration;
using RentMovie.Application.Commands;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Handlers;

public class CreateOrderCommandHandler : IOrderDrivingPort
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;
    private readonly IConfiguration _configuration;

    public CreateOrderCommandHandler(IDatabaseDrivenPort databaseDrivenPort,
        IConfiguration configuration)
    {
        _databaseDrivenPort = databaseDrivenPort;
        _configuration = configuration;
    }

    public async Task<Order> Handle(CreateOrderCommand command)
    {
        var cart = new Cart(command.CustomerCartBytes);
        var originZipCode = _configuration["Shipping:OriginZipCode"];
        var user = await _databaseDrivenPort.GetUserAsync(command.Username);

        var order = new Order(user, cart.TotalPrice, command.PaymentMethod);
        order.LoadShippingPrice(originZipCode);
        order.LoadTotalPrice();

        var entity = await _databaseDrivenPort.AddOrderAsync(order);

        return entity;
    }

    public Task Handle(FinishOrderCommand command)
    {
        throw new NotImplementedException();
    }
}
