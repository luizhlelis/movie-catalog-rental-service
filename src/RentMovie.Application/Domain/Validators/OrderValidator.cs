using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class OrderValidator : AbstractValidator<OrderDto>
{
    public OrderValidator(IDatabaseDrivenPort databaseDrivenPort,
        IHttpContextAccessor contextAccessor,
        IDistributedCache cache)
    {
        var claim = contextAccessor.HttpContext?.User.FindFirst(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var username = claim?.Value ?? "";

        RuleFor(orderDto => orderDto)
            .NotEmpty()
            .MustAsync(CustomerOrderAlreadyRegistered)
            .WithMessage("There is no order registered with that Id")
            .WithErrorCode(ErrorCode.NotFound);

        RuleFor(orderDto => orderDto)
            .NotEmpty()
            .MustAsync(HaveCartInCache)
            .WithMessage("Cart not found or expired, you must create it first")
            .WithErrorCode(ErrorCode.NotFound);

        async Task<bool> CustomerOrderAlreadyRegistered(OrderDto orderDto,
            CancellationToken cancellationToken)
        {
            var order = await databaseDrivenPort.GetOrderByIdAsync(orderDto.OrderId);

            return order is not null && username == order.Customer.Username &&
                   order.Status is OrderStatus.Created;
        }

        async Task<bool> HaveCartInCache(OrderDto orderDto, CancellationToken cancellationToken)
        {
            var cartContent = await cache.GetAsync(username);
            return cartContent is not null;
        }
    }
}
