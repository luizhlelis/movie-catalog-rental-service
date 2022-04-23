using System.ComponentModel.DataAnnotations;

namespace RentMovie.Application.Domain.Entities;

public class Actor
{
    [Key] [Required] public string Name { get; private set; }
}
