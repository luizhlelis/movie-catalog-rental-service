using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation.Results;

namespace RentMovie.Application.Domain;

public abstract class Validatable
{
    [NotMapped] public ValidationResult? ValidationResult { get; set; }

    public abstract bool IsValid();
}
