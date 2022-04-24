using RentMovie.Application.Commands;

namespace RentMovie.Application.Ports;

public interface ICartDrivingPort
{
    public Task Handle(AddItemToCartCommand command);
}
