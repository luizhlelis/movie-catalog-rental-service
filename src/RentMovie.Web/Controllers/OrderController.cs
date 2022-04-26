using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class OrderController : BaseController
{
    private readonly IOrderDrivingPort _orderHandler;
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public OrderController(IOrderDrivingPort orderHandler,
        IDatabaseDrivenPort databaseDrivenPort)
    {
        _orderHandler = orderHandler;
        _databaseDrivenPort = databaseDrivenPort;
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> GetOrder([FromQuery] GetOrderDto getOrderDto)
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

        return response is null
            ? BadRequest(new BadRequestResponse("Your cart is empty!",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : Created($"v1/order/{response.Id}", response);
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
            ? BadRequest(new BadRequestResponse(
                "Some of the items in your cart are not available anymore, please delete your cart and start again",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier))
            : NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrder([FromQuery] OrderDto deleteOrderDto)
    {
        return Ok(await _databaseDrivenPort.DeleteOrderAsync(deleteOrderDto.OrderId));
    }
}
