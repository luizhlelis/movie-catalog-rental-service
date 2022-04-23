using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Dtos;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Ports;
using RentMovie.Web.Filters;
using RentMovie.Web.Responses;

namespace RentMovie.Web.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("v{version:apiVersion}/[controller]")]
public class UserController : ControllerBase
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public UserController(IDatabaseDrivenPort databaseDrivenPort)
    {
        _databaseDrivenPort = databaseDrivenPort;
    }

    [AuthorizeOnly(Role.Admin)]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string username)
    {
        var user = await _databaseDrivenPort.GetUserAsync(username);

        return user is null
            ? NotFound(new NotFoundResponse("User not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Ok(user);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var requesterUsername =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var user = await _databaseDrivenPort.GetUserAsync(requesterUsername?.Value ?? "");

        return user is null
            ? NotFound(new NotFoundResponse("User not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        var userToCreate = new User(userDto.Username, userDto.Password);
        var user = await _databaseDrivenPort.AddUserAsync(userToCreate);
        return Created("v1/user/me", user);
    }

    [AuthorizeOnly(Role.Admin)]
    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdminUser(UserDto userDto)
    {
        var userToCreate = new User(userDto.Username, userDto.Password, Role.Admin);
        var user = await _databaseDrivenPort.AddUserAsync(userToCreate);
        return Created("v1/user/", user);
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var requesterUsername =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var user = await _databaseDrivenPort.GetUserAsync(requesterUsername?.Value ?? "");

        if (user is null)
            return NotFound(new NotFoundResponse("User has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        await _databaseDrivenPort.DeleteUserAsync(user);
        return Ok();
    }

    [AuthorizeOnly(Role.Admin)]
    [HttpDelete("{username}")]
    public async Task<IActionResult> Delete(string username)
    {
        var userToDelete = await _databaseDrivenPort.GetUserAsync(username);

        if (userToDelete is null)
            return NotFound(new NotFoundResponse("User has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        await _databaseDrivenPort.DeleteUserAsync(userToDelete);
        return Ok();
    }
}
