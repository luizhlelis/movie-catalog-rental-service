using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Ports;

public interface IDatabaseDrivenPort
{
    Task<User?> GetUserAsync(string username);
}
