using System.ComponentModel.DataAnnotations;

namespace RentMovie.Application.Domain.Entities;

public class RentCategory
{
    public RentCategory(string name, double price)
    {
        Name = name;
        Price = price;
    }

    // Empty constructor required for EF
    public RentCategory() { }

    [Key] [Required] [MaxLength(20)] public string Name { get; set; }

    [Required] public double Price { get; set; }

    public virtual ICollection<Movie> Movies { get; set; }
}
