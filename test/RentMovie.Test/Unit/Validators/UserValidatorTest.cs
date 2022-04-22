using System.Linq;
using FluentAssertions;
using RentMovie.Application.Dtos;
using RentMovie.Application.Domain.Validators;
using Xunit;

namespace RentMovie.Test.Unit.Validators;

public class UserValidatorTest
{
    private const string ValidPassword = "Abcd@123";
    private const string ValidUsername = "valid-username";

    [Fact(DisplayName = "Should return true when username has all requirements")]
    public void Validate_WhenHasAllRequirements_ShouldReturnTrue()
    {
        // arrange
        var user = new UserDto {Username = ValidUsername, Password = ValidPassword};

        // act
        var validationResult = new UserValidator().Validate(user);

        // assert
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Theory(DisplayName = "Should return false when username is null or empty")]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WhenNullOrEmptyUsername_ShouldReturnFalse(string username)
    {
        // arrange
        var user = new UserDto {Username = username, Password = ValidPassword};

        // act
        var validationResult = new UserValidator().Validate(user);

        // assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should()
            .Be("'Username' must not be empty.");
    }

    [Fact(DisplayName = "Should return false when username is bigger than twenty characters")]
    public void Validate_WhenUsernameBiggerThanTwenty_ShouldReturnFalse()
    {
        // arrange
        var user = new UserDto {Username = "assasasasasasasasasasasasas", Password = ValidPassword};

        // act
        var validationResult = new UserValidator().Validate(user);

        // assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should()
            .Be(
                "The length of 'Username' must be 20 characters or fewer. You entered 27 characters.");
    }

    [Fact(DisplayName = "Should return false when password has no uppercase character")]
    public void Validate_WhenHasNoUppercaseCharacter_ShouldReturnFalse()
    {
        // arrange
        var user = new UserDto {Username = ValidUsername, Password = "abcd@123"};

        // act
        var validationResult = new UserValidator().Validate(user);

        // assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should()
            .Be("Password must have at least one uppercase letter");
    }

    [Fact(DisplayName = "Should return false when password does not have minimum lenght")]
    public void Validate_WhenDoesNotHaveMinimumLenght_ShouldReturnFalse()
    {
        // arrange
        var user = new UserDto {Username = ValidUsername, Password = "Abcd@12"};

        // act
        var validationResult = new UserValidator().Validate(user);

        // assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should()
            .Be("The length of Password must be at least 8 characters");
    }

    [Fact(DisplayName = "Should return false when password has no number")]
    public void Validate_WhenHasNoNumber_ShouldReturnFalse()
    {
        // arrange
        var user = new UserDto {Username = ValidUsername, Password = "Abcd@abcd"};

        // act
        var validationResult = new UserValidator().Validate(user);

        // assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should()
            .Be("Password must have at least one number");
    }

    [Fact(DisplayName = "Should return false when password has no special character")]
    public void Validate_WhenHasNoSpecialCharacter_ShouldReturnFalse()
    {
        // arrange
        var user = new UserDto {Username = ValidUsername, Password = "Abcda123"};

        // act
        var validationResult = new UserValidator().Validate(user);

        // assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.First().ErrorMessage.Should()
            .Be("Password must have at least one special character: !@#$&*");
    }
}