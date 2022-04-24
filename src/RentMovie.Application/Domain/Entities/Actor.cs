using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace RentMovie.Application.Domain.Entities;

public class Actor
{
    public Actor(string name)
    {
        Name = name;
    }

    // Empty constructor required for EF
    public Actor() { }

    [Key] [Required] [MaxLength(30)] public string Name { get; set; }

    [JsonIgnore] [IgnoreDataMember] public virtual ICollection<Movie>? Movies { get; set; }
}
