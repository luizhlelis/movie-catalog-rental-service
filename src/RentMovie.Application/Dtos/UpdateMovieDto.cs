using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Dtos;

public class UpdateMovieDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Synopsis { get; set; }

    public int ReleaseYear { get; set; }

    public int AmountAvailable { get; set; }

    public virtual MovieCategory? Category { get; set; }

    public virtual RentCategory? RentCategory { get; set; }

    public virtual ICollection<Actor>? Cast { get; set; }

    public virtual Director? Director { get; set; }
}
