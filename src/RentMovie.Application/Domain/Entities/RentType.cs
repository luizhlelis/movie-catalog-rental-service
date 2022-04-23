using System.ComponentModel.DataAnnotations;

namespace RentMovie.Application.Domain.Entities;

public class RentType
{
    [Key] [Required] [MaxLength(20)] public string Name { get; private set; }

    [Required] public double Price { get; private set; }

    public virtual ICollection<Movie> Movies { get; set; }
}
