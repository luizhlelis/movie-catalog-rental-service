using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Ports;

namespace RentMovie.Web.Controllers;

public class BaseController : ControllerBase
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public BaseController(IDatabaseDrivenPort databaseDrivenPort)
    {
        _databaseDrivenPort = databaseDrivenPort;
    }

    public async Task<User?> GetRequesterUserAsync()
    {
        var requesterUsername =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        return await _databaseDrivenPort.GetUserAsync(requesterUsername?.Value ?? "");
    }
}
