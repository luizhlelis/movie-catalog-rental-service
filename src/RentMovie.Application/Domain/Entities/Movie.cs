using System.ComponentModel.DataAnnotations;

namespace RentMovie.Application.Domain.Entities;

public class Movie
{
    [Required] public Guid Id { get; protected set; }

    [Required] public string Title { get; private set; }

    [Required] public string Synopsis { get; private set; }

    [Required] public DateTime ReleaseYear { get; private set; }

    [Required] public string Category { get; private set; }

    public virtual RentType RentType { get; private set; }

    public virtual ICollection<Actor> Cast { get; set; }
}
