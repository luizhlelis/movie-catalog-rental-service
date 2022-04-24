using System.Collections.Generic;
using Bogus;
using RentMovie.Application.Domain.Entities;

namespace RentMovie.Test.Helpers;

public static class Fakers
{
    public static List<Director> GetValidDirectors()
    {
        return new List<Director>
            {new("Kleber Mendonça Filho"), new("Spike Lee"), new("Kathryn Bigelow")};
    }

    public static List<Actor> GetValidCast()
    {
        return new List<Actor>
            {new("Leonardo Dicaprio"), new("Fernanda Montenegro"), new("Viola Davis")};
    }

    public static List<MovieCategory> GetValidMovieCategories()
    {
        return new List<MovieCategory>
            {new("Drama"), new("Horror"), new("Romance"), new("Comedy")};
    }

    public static List<RentCategory> GetValidRentCategories()
    {
        return new List<RentCategory>
            {new("Release", 5), new("Requested", 2), new("Old", 1)};
    }

    public static Movie GetValidMovie()
    {
        var faker = new Faker<Movie>("en_US")
            .RuleFor(movie => movie.Title, faker => faker.Lorem.Word())
            .RuleFor(movie => movie.Synopsis, faker => faker.Lorem.Sentence(4))
            .RuleFor(movie => movie.ReleaseYear, faker => faker.Date.Recent().Year)
            .RuleFor(movie => movie.Category, faker => faker.PickRandom(GetValidMovieCategories()))
            .RuleFor(movie => movie.RentCategory,
                faker => faker.PickRandom(GetValidRentCategories()))
            .RuleFor(movie => movie.Cast, GetValidCast())
            .RuleFor(movie => movie.Director, faker => faker.PickRandom(GetValidDirectors()));

        return faker.Generate();
    }
}
