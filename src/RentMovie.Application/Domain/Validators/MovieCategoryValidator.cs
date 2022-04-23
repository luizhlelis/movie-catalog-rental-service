using FluentValidation;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Domain.Validators;

public class MovieCategoryValidator : AbstractValidator<MovieCategory>
{
    public MovieCategoryValidator()
    {
        RuleFor(movieCategory => movieCategory.Name)
            .NotEmpty()
            .MaximumLength(20);
    }
}
