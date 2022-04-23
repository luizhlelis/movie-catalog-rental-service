using FluentValidation;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator(IDatabaseDrivenPort databaseDrivenPort)
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

        RuleFor(user => user.Username)
            .MustAsync(HasNotYetBeenRegistered)
            .WithMessage("User has already been registered");

        async Task<bool> HasNotYetBeenRegistered(string username,
            CancellationToken cancellationToken)
        {
            var user = await databaseDrivenPort.GetUserAsync(username);
            return user is null;
        }
    }
}
