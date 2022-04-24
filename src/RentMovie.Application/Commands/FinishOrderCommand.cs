using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;

namespace RentMovie.Application.Commands;

public class FinishOrderCommand
{
    public Guid OrderId { get; set; }

    public string Username { get; set; }
}
