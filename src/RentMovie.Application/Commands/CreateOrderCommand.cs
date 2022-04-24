using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;

namespace RentMovie.Application.Commands;

public class CreateOrderCommand
{
    public string Username { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
}
