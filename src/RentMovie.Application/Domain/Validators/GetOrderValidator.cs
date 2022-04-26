using FluentValidation;
using Microsoft.AspNetCore.Http;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class GetOrderValidator : AbstractValidator<GetOrderDto>
{
    public GetOrderValidator(IDatabaseDrivenPort databaseDrivenPort,
        IHttpContextAccessor contextAccessor)
    {
        var claim = contextAccessor.HttpContext?.User.FindFirst(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var username = claim?.Value ?? "";

        RuleFor(getOrderDto => getOrderDto)
            .NotEmpty()
            .MustAsync(OrderAlreadyRegistered)
            .WithMessage("There is no order registered with that Id")
            .WithErrorCode(ErrorCode.NotFound);

        async Task<bool> OrderAlreadyRegistered(GetOrderDto getOrderDto,
            CancellationToken cancellationToken)
        {
            var order = await databaseDrivenPort.GetOrderByIdAsync(getOrderDto.OrderId);

            return order is not null && username == order.Customer.Username;
        }
    }
}
