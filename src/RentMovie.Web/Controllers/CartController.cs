using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Commands;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;
using RentMovie.Web.Filters;
using RentMovie.Web.Responses;

namespace RentMovie.Web.Controllers;

[ApiController]
[Authorize]
[AuthorizeOnly(Role.Customer)]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class CartController : BaseController
{
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ICartDrivingPort _cartHandler;

    public CartController(IDistributedCache cache, IConfiguration configuration,
        ICartDrivingPort cartHandler)
    {
        _cache = cache;
        _configuration = configuration;
        _cartHandler = cartHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var username = GetUsernameFromToken();
        var response = await _cache.GetAsync(username);

        if (response is null)
            return NotFound(new NotFoundResponse("There is no cart registered.",
                Activity.Current?.Id ?? ""));

        return Ok(new Cart(response));
    }

    [HttpPost]
    public async Task<IActionResult> PostCreateCart()
    {
        var username = GetUsernameFromToken();
        var cart = new Cart();
        var expireIn = _configuration.GetValue<int>("Cache:ExpirationInHoursRelativeToNow");

        var cartContent = await _cache.GetAsync(username);

        if (cartContent is not null)
            return BadRequest(new BadRequestResponse(
                "There is another cart registered, please delete it first",
                Activity.Current?.Id ?? ""));

        await _cache.SetAsync(
            username,
            cart.ToBytes(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expireIn)
            });

        return Created("v1/cart", cart);
    }

    [HttpPut]
    public async Task<IActionResult> PutAddMovieToCart([FromBody] CartDto cartDto)
    {
        var username = GetUsernameFromToken();

        await _cartHandler.Handle(new AddItemToCartCommand
        {
            Username = username, MovieId = cartDto.MovieId
        });

        return NoContent();
    }
}
