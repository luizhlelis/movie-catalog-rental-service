using FluentValidation;
using RentMovie.Application.Dtos;

namespace RentMovie.Application.Domain.Validators;

public class MoviesDtoValidator : AbstractValidator<MoviesDto>
{
    public MoviesDtoValidator()
    {
        RuleFor(dto => dto.Page)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);

        RuleFor(dto => dto.PageSize)
            .NotEmpty()
            .LessThanOrEqualTo(10);
    }
}
