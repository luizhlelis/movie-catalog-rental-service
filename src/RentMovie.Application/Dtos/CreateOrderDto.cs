using RentMovie.Application.Domain.Enums;

namespace RentMovie.Application.Dtos;

public class CreateOrderDto
{
    public PaymentMethod PaymentMethod { get; set; }
}
