using FluentValidation;

namespace RentMovie.Application.Domain.ValueObjects;

public class Password : Validatable
{
    public Password(string password)
    {
        PlainText = password;
        Hash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string Hash { get; }

    public string PlainText { get; }

    public override bool IsValid()
    {
        ValidationResult = new PasswordValidator().Validate(this);
        return ValidationResult.IsValid;
    }

    public bool Verify(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, Hash);
    }
}

public class PasswordValidator : AbstractValidator<Password>
{
    public PasswordValidator()
    {
        RuleFor(password => password.PlainText)
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("Password must have at least one uppercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must have at least one number")
            .Matches("[!@#$&*]")
            .WithMessage("Password must have at least one special character: !@#$&*");
    }
}
