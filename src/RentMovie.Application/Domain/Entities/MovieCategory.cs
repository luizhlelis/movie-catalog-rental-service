using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace RentMovie.Application.Domain.Entities;

public class MovieCategory
{
    public MovieCategory(string name)
    {
        Name = name;
    }

    // Empty constructor required for EF
    public MovieCategory() { }

    [Key] [Required] [MaxLength(20)] public string Name { get; set; }

    [JsonIgnore] [IgnoreDataMember] public virtual ICollection<Movie>? Movies { get; set; }
}
