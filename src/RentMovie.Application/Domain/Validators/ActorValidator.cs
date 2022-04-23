using FluentValidation;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Domain.Validators;

public class CastValidator : AbstractValidator<ICollection<Actor>>
{
    public CastValidator()
    {
        RuleForEach(actor => actor)
            .SetValidator(new ActorValidator());
    }
}

public class ActorValidator : AbstractValidator<Actor>
{
    public ActorValidator()
    {
        RuleFor(actor => actor.Name)
            .NotEmpty()
            .MaximumLength(30);
    }
}
