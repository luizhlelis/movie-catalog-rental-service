using FluentValidation;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.ValueObjects;

/*  Resource Owner Password Credentials grant type was deprecated by OAuth2.0,
    this should be avoided in a production environment */

public class OwnerCredential
{
    public string Username { get; set; }

    public string Password { get; set; }
}

public class OwnerCredentialsValidator : AbstractValidator<OwnerCredential>
{
    public OwnerCredentialsValidator(IDatabaseDrivenPort databaseDrivenPort)
    {
        RuleFor(credentials => credentials.Username)
            .NotEmpty();

        RuleFor(credentials => credentials.Password)
            .NotEmpty();

        RuleFor(credentials => credentials)
            .NotEmpty()
            .MustAsync(AreCredentialsValid)
            .WithMessage("User or password mismatch")
            .WithErrorCode(ErrorCode.Forbidden);

        async Task<bool> AreCredentialsValid(OwnerCredential credentials,
            CancellationToken cancellationToken)
        {
            var incomingPasswordHash = new Password(credentials.Password).Hash;
            var user = await databaseDrivenPort.GetUserAsync(credentials.Username);

            return user is not null && incomingPasswordHash == user.PasswordHash;
        }
    }
}
