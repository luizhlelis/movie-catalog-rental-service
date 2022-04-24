using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class CartDtoValidator : AbstractValidator<CartDto>
{
    public CartDtoValidator(IDatabaseDrivenPort databaseDrivenPort,
        IHttpContextAccessor contextAccessor,
        IDistributedCache cache)
    {
        var claim = contextAccessor.HttpContext?.User.FindFirst(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var username = claim?.Value ?? "";

        RuleFor(cart => cart.MovieId)
            .NotEmpty()
            .MustAsync(MovieAlreadyRegisteredAndAvailable)
            .WithMessage("Movie not found or not available to rent")
            .WithErrorCode(ErrorCode.NotFound);

        RuleFor(cart => cart)
            .NotEmpty()
            .MustAsync(HaveCartInCache)
            .WithMessage("Cart not found or expired, you must create it first")
            .WithErrorCode(ErrorCode.NotFound);

        async Task<bool> MovieAlreadyRegisteredAndAvailable(Guid movieId,
            CancellationToken cancellationToken)
        {
            var movie = await databaseDrivenPort.GetMovieByIdAsync(movieId);

            return movie is not null && movie.AmountAvailable > 0;
        }

        async Task<bool> HaveCartInCache(CartDto cartDto, CancellationToken cancellationToken)
        {
            var cartContent = await cache.GetAsync(username);
            return cartContent is not null;
        }
    }
}
