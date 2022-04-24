using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Commands;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;
using RentMovie.Web.Filters;

namespace RentMovie.Web.Controllers;

[ApiController]
[Authorize]
[AuthorizeOnly(Role.Customer)]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class OrderController : BaseController
{
    private readonly IDistributedCache _cache;
    private readonly IOrderDrivingPort _orderHandler;
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public OrderController(IDistributedCache cache, IOrderDrivingPort orderHandler,
        IDatabaseDrivenPort databaseDrivenPort)
    {
        _cache = cache;
        _orderHandler = orderHandler;
        _databaseDrivenPort = databaseDrivenPort;
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] OrderDto getOrderDto)
    {
        return Ok(await _databaseDrivenPort.GetOrderByIdAsync(getOrderDto.OrderId));
    }

    [HttpPost]
    public async Task<IActionResult> PostCreateOrder([FromBody] CreateOrderDto orderDto)
    {
        var username = GetUsernameFromToken();

        var response = await _orderHandler.Handle(new CreateOrderCommand
        {
            PaymentMethod = orderDto.PaymentMethod, Username = username
        });

        return Created($"v1/order/{response.Id}", response);
    }

    [HttpPut("finish")]
    public async Task<IActionResult> PutFinishOrder([FromBody] OrderDto finishOrderDto)
    {
        var username = GetUsernameFromToken();

        var result = await _orderHandler.Handle(new FinishOrderCommand
        {
            OrderId = finishOrderDto.OrderId, Username = username
        });

        return result is null
            ? BadRequest(
                "Some of the items in your cart are not available anymore, please delete your cart and start again")
            : NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrder([FromQuery] OrderDto deleteOrderDto)
    {
        return Ok(await _databaseDrivenPort.DeleteOrderAsync(deleteOrderDto.OrderId));
    }
}
