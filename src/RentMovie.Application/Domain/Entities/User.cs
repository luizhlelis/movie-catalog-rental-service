using FluentValidation;
using RentMovie.Application.Domain.ValueObjects;

namespace RentMovie.Application.Domain.Entities;

public class User : Validatable
{
    public User(string username, Password password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; }

    public Password Password { get; }

    public override bool IsValid()
    {
        ValidationResult = new UserValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty()
            .MaximumLength(20);
    }
}
