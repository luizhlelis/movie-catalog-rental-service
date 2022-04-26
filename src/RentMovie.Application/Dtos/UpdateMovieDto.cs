using RentMovie.Application.Domain.Entities;

namespace RentMovie.Application.Dtos;

public class UpdateMovieDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Synopsis { get; set; }

    public int ReleaseYear { get; set; }

    public int AmountAvailable { get; set; }
}
