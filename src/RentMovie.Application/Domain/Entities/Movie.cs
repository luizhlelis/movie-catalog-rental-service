using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;

namespace RentMovie.Application.Domain.Entities;

[Index(nameof(Id), nameof(Title), IsUnique = true)]
public class Movie : IdentifiableEntity
{
    public Movie(string title, string synopsis, int releaseYear, int amountAvailable)
    {
        Title = title;
        Synopsis = synopsis;
        ReleaseYear = releaseYear;
        AmountAvailable = amountAvailable;
    }

    // Empty constructor required for EF
    public Movie() { }

    [Required] [MaxLength(50)] public string Title { get; set; }

    [Required] [MaxLength(600)] public string Synopsis { get; set; }

    [Required] public int ReleaseYear { get; set; }

    [Required] public int AmountAvailable { get; set; }

    public virtual MovieCategory? Category { get; set; }

    public virtual RentCategory? RentCategory { get; set; }

    public virtual ICollection<Actor>? Cast { get; set; }

    public virtual Director? Director { get; set; }

    public void Bind(UpdateMovieDto movie)
    {
        Title = movie.Title;
        Synopsis = movie.Synopsis;
        ReleaseYear = movie.ReleaseYear;
        AmountAvailable = movie.AmountAvailable;
    }

    public Movie ShallowCopy()
    {
        return (Movie)MemberwiseClone();
    }

    public void RemoveNestedProperties()
    {
        Category = null;
        RentCategory = null;
        Director = null;
        Cast = null;
    }

    public void RemoveAmountFromStock(int quantity)
    {
        AmountAvailable -= quantity;
    }
}
