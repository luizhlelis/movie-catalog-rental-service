using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;

namespace RentMovie.Application.Commands;

public class FinishOrderCommand
{
    public string Username { get; set; }

    public byte[] CustomerCartBytes { get; set; }
}
