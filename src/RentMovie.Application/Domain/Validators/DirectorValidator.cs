using FluentValidation;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Domain.Validators;

public class DirectorValidator : AbstractValidator<Director>
{
    public DirectorValidator()
    {
        RuleFor(director => director.Name)
            .NotEmpty()
            .MaximumLength(30);
    }
}
