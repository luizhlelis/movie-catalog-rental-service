using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Commands;
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
public class OrderController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly IOrderDrivingPort _orderHandler;

    public OrderController(IDistributedCache cache, IOrderDrivingPort orderHandler)
    {
        _cache = cache;
        _orderHandler = orderHandler;
    }

    // GET
    [HttpGet("{id}")]
    public IActionResult Index()
    {
        // check if not null and if order is from the requester username
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> PostCreateOrder([FromBody] PaymentMethod paymentMethod)
    {
        var username =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var cartContent = await _cache.GetAsync(username?.Value ?? "");

        if (cartContent is null)
            return NotFound(new NotFoundResponse(
                "Cart not found or expired, you must create it first",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));

        var response = await _orderHandler.Handle(new CreateOrderCommand
        {
            CustomerCartBytes = cartContent,
            PaymentMethod = paymentMethod,
            Username = username?.Value ?? ""
        });

        return Created("v1/order/{id}", response);
    }

    [HttpPut("finish")]
    public IActionResult PutFinishOrder([FromBody] OrderDto finishOrderDto)
    {
        return NoContent();
    }

    [HttpDelete]
    public IActionResult DeleteOrder()
    {
        // check if not null and if order is from the requester username and order status
        return Ok();
    }
}
