using FluentValidation;
using Microsoft.AspNetCore.Http;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class OrderValidator : AbstractValidator<OrderDto>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public OrderValidator(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public OrderValidator(IDatabaseDrivenPort databaseDrivenPort)
    {
        var claim = _contextAccessor?.HttpContext?.User.FindFirst(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var username = claim?.Value ?? "";

        RuleFor(finishOrder => finishOrder.OrderId)
            .NotEmpty()
            .MustAsync(CustomerOrderAlreadyRegistered)
            .WithMessage("There is no order registered with that Id")
            .WithErrorCode(ErrorCode.NotFound);

        async Task<bool> CustomerOrderAlreadyRegistered(Guid orderId,
            CancellationToken cancellationToken)
        {
            var order = await databaseDrivenPort.GetOrderByIdAsync(orderId);

            return order is not null && username == order.Customer.Username &&
                   order.Status is OrderStatus.Created;
        }
    }
}
