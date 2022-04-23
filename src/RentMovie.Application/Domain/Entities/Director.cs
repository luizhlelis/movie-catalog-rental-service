using System.ComponentModel.DataAnnotations;

namespace RentMovie.Application.Domain.Entities;

public class Director
{
    public Director(string name)
    {
        Name = name;
    }

    // Empty constructor required for EF
    public Director() { }

    [Key] [Required] [MaxLength(30)] public string Name { get; set; }

    public virtual ICollection<Movie> Movies { get; set; }
}
