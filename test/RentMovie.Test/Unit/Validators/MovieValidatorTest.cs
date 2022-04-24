using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Bogus;
using FluentAssertions;
using Moq;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Validators;
using RentMovie.Application.Ports;
using RentMovie.Test.Helpers;
using Xunit;

namespace RentMovie.Test.Unit.Validators;

public class MovieValidatorTest
{
    private readonly IDatabaseDrivenPort _mockedDatabase;

    public MovieValidatorTest()
    {
        var mockedDatabaseAdapter = new Mock<IDatabaseDrivenPort>();
        mockedDatabaseAdapter
            .Setup(x => x.GetMovieByExpressionAsync(It.IsAny<Expression<Func<Movie, bool>>>()))
            .ReturnsAsync(new List<Movie>());
        _mockedDatabase = mockedDatabaseAdapter.Object;
    }

    [Fact(DisplayName = "Should return true when movie has all requirements")]
    public void ValidateMovie_WhenMovieHasAllRequirements_ShouldReturnTrue()
    {
        // arrange
        var movie = Fakers.GetValidMovie();

        // act
        var validationResult = new MovieValidator(_mockedDatabase).Validate(movie);

        // assert
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
