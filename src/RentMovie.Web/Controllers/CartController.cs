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
public class CartController : ControllerBase
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
        var username =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var response = await _cache.GetAsync(username?.Value ?? "");

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> PostCreateCart()
    {
        var username =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var cart = new Cart();
        var expireIn = _configuration.GetValue<int>("Cache:ExpirationInHoursRelativeToNow");

        await _cache.SetAsync(
            username?.Value ?? "",
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
        var username =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var cartContent = await _cache.GetAsync(username?.Value ?? "");

        if (cartContent is null)
            return NotFound(new NotFoundResponse(
                "Cart not found or expired, you must create it first",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        await _cartHandler.Handle(new AddItemToCartCommand
        {
            Username = username?.Value, MovieId = cartDto.MovieId
        });

        return NoContent();
    }
}
