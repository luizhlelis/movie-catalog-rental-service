﻿using System.Diagnostics;
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
public class UserController : BaseController
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public UserController(IDatabaseDrivenPort databaseDrivenPort)
    {
        _databaseDrivenPort = databaseDrivenPort;
    }

    [AuthorizeOnly(Role.Admin)]
    [HttpGet("{username}")]
    public async Task<IActionResult> Get(string username)
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
        var requesterUsername = GetUsernameFromToken();
        var user = await _databaseDrivenPort.GetUserAsync(requesterUsername);

        return user is null
            ? NotFound(new NotFoundResponse("User not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        var userToCreate = new User(userDto.Username, userDto.Password, userDto.ZipCode);
        var user = await _databaseDrivenPort.AddUserAsync(userToCreate);
        return Created("v1/user/me", user);
    }

    [AuthorizeOnly(Role.Admin)]
    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdminUser(UserDto userDto)
    {
        var userToCreate =
            new User(userDto.Username, userDto.Password, userDto.ZipCode, Role.Admin);
        var user = await _databaseDrivenPort.AddUserAsync(userToCreate);
        return Created("v1/user/{username}", user);
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var requesterUsername = GetUsernameFromToken();
        var user = await _databaseDrivenPort.GetUserAsync(requesterUsername);

        if (user is null)
            return NotFound(new NotFoundResponse("User has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        var response = await _databaseDrivenPort.DeleteUserAsync(user);
        return Ok(response);
    }

    [AuthorizeOnly(Role.Admin)]
    [HttpDelete("{username}")]
    public async Task<IActionResult> Delete(string username)
    {
        var userToDelete = await _databaseDrivenPort.GetUserAsync(username);

        if (userToDelete is null)
            return NotFound(new NotFoundResponse("User has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        var response = await _databaseDrivenPort.DeleteUserAsync(userToDelete);
        return Ok(response);
    }
}
