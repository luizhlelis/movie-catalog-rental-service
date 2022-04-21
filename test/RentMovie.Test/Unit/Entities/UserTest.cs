using System.Linq;
using FluentAssertions;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.ValueObjects;
using Xunit;

namespace RentMovie.Test.Unit.Entities;

public class UserTest
{
    private readonly Password _validPassword;

    public UserTest()
    {
        _validPassword = new Password("Abcd@123");
    }

    [Fact(DisplayName = "Should return true when username has all requirements")]
    public void UserIsValid_WhenHasAllRequirements_ShouldReturnTrue()
    {
        var username = "username";

        var password = new User(username, _validPassword);

        password.IsValid().Should().BeTrue();
        password.ValidationResult?.Errors.Should().BeEmpty();
    }

    [Theory(DisplayName = "Should return false when username is null or empty")]
    [InlineData(null)]
    [InlineData("")]
    public void UserIsValid_WhenNullOrEmptyUsername_ShouldReturnFalse(string username)
    {
        var password = new User(username, _validPassword);

        password.IsValid().Should().BeFalse();
        password.ValidationResult?.Errors.First().ErrorMessage.Should()
            .Be("'Username' must not be empty.");
    }
}
