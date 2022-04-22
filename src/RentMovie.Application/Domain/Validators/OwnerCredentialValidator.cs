using FluentValidation;
using RentMovie.Application.Domain.Dtos;
using RentMovie.Application.Domain.ValueObjects;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Validators;

public class OwnerCredentialValidator : AbstractValidator<OwnerCredentialDto>
{
    public OwnerCredentialValidator(IDatabaseDrivenPort databaseDrivenPort)
    {
        RuleFor(credentials => credentials.Username)
            .NotEmpty();

        RuleFor(credentials => credentials.Password)
            .NotEmpty();

        RuleFor(credentials => credentials)
            .MustAsync(AreCredentialsValid)
            .WithMessage("User or password mismatch")
            .WithErrorCode(ErrorCode.Forbidden);

        async Task<bool> AreCredentialsValid(OwnerCredentialDto credentials,
            CancellationToken cancellationToken)
        {
            var incomingPassword = new Password(credentials.Password);
            var user = await databaseDrivenPort.GetUserAsync(credentials.Username);

            return user is not null && incomingPassword.HasHashMatchWith(user.PasswordHash);
        }
    }
}
