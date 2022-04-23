using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Ports;
using RentMovie.Web.Filters;
using RentMovie.Web.Responses;

namespace RentMovie.Web.Controllers;

[ApiController]
[Authorize]
[AuthorizeOnly(Role.Admin)]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class MovieController : ControllerBase
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public MovieController(IDatabaseDrivenPort databaseDrivenPort)
    {
        _databaseDrivenPort = databaseDrivenPort;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid movieId)
    {
        var movie = await _databaseDrivenPort.GetMovieByIdAsync(movieId);

        return movie is null
            ? NotFound(new NotFoundResponse("Movie not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Ok(movie);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Movie movie)
    {
        var response = await _databaseDrivenPort.AddMovieAsync(movie);

        return Created("v1/movie/", response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] Movie inputMovie)
    {
        var movie = await _databaseDrivenPort.GetMovieByIdAsync(id);

        if (movie is null)
            return NotFound(new NotFoundResponse("Movie not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        movie.Bind(inputMovie);

        await _databaseDrivenPort.UpdateMovieAsync(movie);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var movie = await _databaseDrivenPort.GetMovieByIdAsync(id);

        if (movie is null)
            return NotFound(new NotFoundResponse("Movie has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        await _databaseDrivenPort.DeleteMovieAsync(movie);

        return Ok();
    }
}
