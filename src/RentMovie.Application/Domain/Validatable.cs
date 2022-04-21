using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FluentValidation.Results;

namespace RentMovie.Application.Domain;

public abstract class Validatable
{
    [NotMapped] [JsonIgnore] public ValidationResult? ValidationResult { get; set; }

    public abstract bool IsValid();
}
