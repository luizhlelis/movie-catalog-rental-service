using FluentValidation;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Domain.Validators;

public class RentCategoryValidator : AbstractValidator<RentCategory>
{
    public RentCategoryValidator()
    {
        RuleFor(rentType => rentType.Name)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(rentType => rentType.Price)
            .GreaterThan(0);
    }
}
