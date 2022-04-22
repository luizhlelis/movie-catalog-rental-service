using FluentValidation;
using RentMovie.Application.Dtos;

namespace RentMovie.Application.Domain.Validators;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(user => user.Password)
            .MinimumLength(8)
            .WithMessage("The length of Password must be at least 8 characters")
            .Matches("[A-Z]")
            .WithMessage("Password must have at least one uppercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must have at least one number")
            .Matches("[!@#$&*]")
            .WithMessage("Password must have at least one special character: !@#$&*");
    }
}
