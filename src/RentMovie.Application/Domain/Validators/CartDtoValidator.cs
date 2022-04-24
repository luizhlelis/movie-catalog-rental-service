using FluentValidation;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class CartDtoValidator : AbstractValidator<CartDto>
{
    public CartDtoValidator(IDatabaseDrivenPort databaseDrivenPort)
    {
        RuleFor(cart => cart.MovieId)
            .MustAsync(MovieAlreadyRegisteredAndAvailable)
            .WithMessage("Movie not found or not available to rent")
            .WithErrorCode(ErrorCode.NotFound);

        async Task<bool> MovieAlreadyRegisteredAndAvailable(Guid movieId,
            CancellationToken cancellationToken)
        {
            var movie = await databaseDrivenPort.GetMovieByIdAsync(movieId);

            return movie is not null && movie.AmountAvailable > 0;
        }
    }
}
