using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Dtos;

namespace RentMovie.Application.Domain.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator(IHttpContextAccessor contextAccessor,
        IDistributedCache cache)
    {
        var claim = contextAccessor.HttpContext?.User.FindFirst(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var username = claim?.Value ?? "";

        RuleFor(orderDto => orderDto)
            .NotEmpty()
            .MustAsync(HaveCartInCache)
            .WithMessage("Cart not found or expired, you must create it first")
            .WithErrorCode(ErrorCode.NotFound);

        async Task<bool> HaveCartInCache(CreateOrderDto orderDto,
            CancellationToken cancellationToken)
        {
            var cartContent = await cache.GetAsync(username);
            return cartContent is not null;
        }
    }
}
