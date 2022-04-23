using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using Moq;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Validators;
using RentMovie.Application.Ports;
using Xunit;

namespace RentMovie.Test.Unit.Validators;

public class MovieValidatorTest
{
    private readonly IDatabaseDrivenPort _mockedDatabase;
    private readonly List<MovieCategory> _validMovieCategories;
    private readonly List<RentCategory> _validRentCategories;
    private readonly List<Actor> _validCast;
    private readonly List<Director> _validDirectors;

    public MovieValidatorTest()
    {
        var mockedDatabaseAdapter = new Mock<IDatabaseDrivenPort>();
        mockedDatabaseAdapter
            .Setup(x => x.GetMovieByExpressionAsync(It.IsAny<Expression<Func<Movie, bool>>>()))
            .ReturnsAsync(new List<Movie>());
        _mockedDatabase = mockedDatabaseAdapter.Object;

        _validMovieCategories = new List<MovieCategory>
            {new("Drama"), new("Horror"), new("Romance"), new("Comedy")};

        _validRentCategories = new List<RentCategory>
            {new("Release", 5), new("Requested", 2), new("Old", 1)};

        _validCast = new List<Actor>
            {new("Leonardo Dicaprio"), new("Fernanda Montenegro"), new("Viola Davis")};

        _validDirectors = new List<Director>
            {new("Kleber Mendonça Filho"), new("Spike Lee"), new("Kathryn Bigelow")};
    }

    [Fact(DisplayName = "Should return true when movie has all requirements")]
    public void ValidateMovie_WhenMovieHasAllRequirements_ShouldReturnTrue()
    {
        // arrange
        var faker = new Faker<Movie>("en_US")
            .RuleFor(movie => movie.Title, faker => faker.Lorem.Word())
            .RuleFor(movie => movie.Synopsis, faker => faker.Lorem.Sentence(4))
            .RuleFor(movie => movie.ReleaseYear, faker => faker.Date.Recent().Year)
            .RuleFor(movie => movie.Category, faker => faker.PickRandom(_validMovieCategories))
            .RuleFor(movie => movie.RentCategory, faker => faker.PickRandom(_validRentCategories))
            .RuleFor(movie => movie.Cast, _validCast)
            .RuleFor(movie => movie.Director, faker => faker.PickRandom(_validDirectors));
        var movie = faker.Generate();

        // act
        var validationResult = new MovieValidator(_mockedDatabase).Validate(movie);

        // assert
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
