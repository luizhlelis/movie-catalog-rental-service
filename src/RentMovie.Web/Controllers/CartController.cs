using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Web.Filters;

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

    public CartController(IDistributedCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var username =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var response = await _cache.GetAsync(username?.Value ?? "");

        return Created("v1/cart", response);
    }

    [HttpPost]
    public async Task<IActionResult> PostCreateCart()
    {
        var username =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        await _cache.SetAsync(
            username?.Value ?? "",
            Encoding.UTF8.GetBytes(""));

        return Created("v1/cart", "");
    }

    [HttpPut]
    public IActionResult PutAddMovieToCart()
    {
        return Ok();
    }
}
