using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;
using RentMovie.Web.Filters;
using RentMovie.Web.Responses;

namespace RentMovie.Web.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class MovieController : ControllerBase
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public MovieController(IDatabaseDrivenPort databaseDrivenPort)
    {
        _databaseDrivenPort = databaseDrivenPort;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var movie = await _databaseDrivenPort.GetMovieByIdIncludingAsync(id);

        return movie is null
            ? NotFound(new NotFoundResponse("Movie not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Ok(movie);
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] MoviesDto moviesDto)
    {
        var movies = await _databaseDrivenPort.GetMoviesAsync(moviesDto.Page, moviesDto.PageSize);

        return Ok(movies);
    }

    [HttpPost]
    [AuthorizeOnly(Role.Admin)]
    public async Task<IActionResult> Post([FromBody] Movie movie)
    {
        var response = await _databaseDrivenPort.AddMovieAsync(movie);

        return Created("v1/movie/{id}", response);
    }

    [HttpPut("{id}")]
    [AuthorizeOnly(Role.Admin)]
    public async Task<IActionResult> Put(Guid id, [FromBody] Movie inputMovie)
    {
        var movie = await _databaseDrivenPort.GetMovieByIdAsync(id);

        if (movie is null)
            return NotFound(new NotFoundResponse("Movie not found",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        movie.Bind(inputMovie);

        await _databaseDrivenPort.UpdateMovieAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [AuthorizeOnly(Role.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var movie = await _databaseDrivenPort.GetMovieByIdAsync(id);

        if (movie is null)
            return NotFound(new NotFoundResponse("Movie has already been deleted",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        var response = await _databaseDrivenPort.DeleteMovieAsync(movie);

        return Ok(response);
    }
}
