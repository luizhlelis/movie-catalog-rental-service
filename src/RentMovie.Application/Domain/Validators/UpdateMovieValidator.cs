using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class UpdateMovieValidator : AbstractValidator<UpdateMovieDto>
{
    public UpdateMovieValidator(IDatabaseDrivenPort databaseDrivenPort)
    {
        RuleFor(movie => movie)
            .MustAsync(HasAlreadyBeenRegistered)
            .WithMessage("Movie not found")
            .WithErrorCode(ErrorCode.NotFound);

        RuleFor(movie => movie.Title)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(movie => movie.Synopsis)
            .NotEmpty()
            .MaximumLength(600);

        RuleFor(movie => movie.ReleaseYear)
            .GreaterThan(1800)
            .LessThanOrEqualTo(DateTime.Now.Year);

        RuleFor(movie => movie.Category)
            .NotNull()
            .SetValidator(new MovieCategoryValidator());

        RuleFor(movie => movie.RentCategory)
            .NotNull()
            .SetValidator(new RentCategoryValidator());

        RuleFor(movie => movie.Cast)
            .NotNull()
            .SetValidator(new CastValidator());

        RuleFor(movie => movie.Director)
            .NotNull()
            .SetValidator(new DirectorValidator());

        async Task<bool> HasAlreadyBeenRegistered(UpdateMovieDto movieToSearch,
            CancellationToken cancellationToken)
        {
            var movies = await databaseDrivenPort.GetMovieByExpressionAsync(movie =>
                movie.Id == movieToSearch.Id || movie.Title == movieToSearch.Title);

            return movies.Any();
        }
    }
}
