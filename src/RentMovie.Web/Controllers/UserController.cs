using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Dtos;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Ports;
using RentMovie.Web.Responses;

namespace RentMovie.Web.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class UserController : BaseController
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public UserController(IDatabaseDrivenPort databaseDrivenPort) : base(databaseDrivenPort)
    {
        _databaseDrivenPort = databaseDrivenPort;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string username)
    {
        var requesterUser = await GetRequesterUserAsync();

        if (requesterUser?.Role is not Role.Admin)
            return Unauthorized();

        var user = await _databaseDrivenPort.GetUserAsync(username);

        return user is null
            ? NotFound(new NotFoundResponse("User not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await GetRequesterUserAsync();

        return user is null
            ? NotFound(new NotFoundResponse("User not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        var userToCreate = new User(userDto.Username, userDto.Password);
        var user = await _databaseDrivenPort.AddUserAsync(userToCreate);
        return Created("/me", user);
    }

    [Authorize]
    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdminUser(UserDto userDto)
    {
        var requesterUser = await GetRequesterUserAsync();

        if (requesterUser?.Role is not Role.Admin)
            return Unauthorized();

        var userToCreate = new User(userDto.Username, userDto.Password, Role.Admin);
        var user = await _databaseDrivenPort.AddUserAsync(userToCreate);
        return Created("/", user);
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var user = await GetRequesterUserAsync();

        if (user is null)
            return NotFound(new NotFoundResponse("User has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        await _databaseDrivenPort.DeleteUserAsync(user);
        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string username)
    {
        var requesterUser = await GetRequesterUserAsync();

        if (requesterUser?.Role is not Role.Admin)
            return Unauthorized();

        var userToDelete = await _databaseDrivenPort.GetUserAsync(username);

        if (userToDelete is null)
            return NotFound(new NotFoundResponse("User has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        await _databaseDrivenPort.DeleteUserAsync(userToDelete);
        return Ok();
    }
}
