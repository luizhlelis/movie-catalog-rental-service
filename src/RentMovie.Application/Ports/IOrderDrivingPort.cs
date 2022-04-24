using RentMovie.Application.Commands;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Ports;

public interface IOrderDrivingPort
{
    public Task<Order> Handle(CreateOrderCommand command);

    public Task Handle(FinishOrderCommand command);
}
