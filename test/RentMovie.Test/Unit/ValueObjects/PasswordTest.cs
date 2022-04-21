using System.Linq;
using FluentAssertions;
using RentMovie.Application.Domain.ValueObjects;
using Xunit;

namespace RentMovie.Test.Unit.ValueObjects;

public class PasswordTest
{
    // MethodUnderTest_Condition_Expectation
    [Fact(DisplayName = "Should return true when password has all requirements")]
    public void PasswordIsValid_WhenHasAllRequirements_ShouldReturnTrue()
    {
        var plainTextPassword = "Abcd@123";

        var password = new Password(plainTextPassword);

        password.IsValid().Should().BeTrue();
        password.ValidationResult?.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should return false when password has no uppercase character")]
    public void PasswordIsValid_WhenHasNoUppercaseCharacter_ShouldReturnFalse()
    {
        var plainTextPassword = "abcd@123";

        var password = new Password(plainTextPassword);

        password.IsValid().Should().BeFalse();
        password.ValidationResult?.Errors.First().ErrorMessage.Should()
            .Be("Password must have at least one uppercase letter");
    }

    [Fact(DisplayName = "Should return false when password does not have minimum lenght")]
    public void PasswordIsValid_WhenDoesNotHaveMinimumLenght_ShouldReturnFalse()
    {
        var plainTextPassword = "Abcd@12";

        var password = new Password(plainTextPassword);

        password.IsValid().Should().BeFalse();
        password.ValidationResult?.Errors.First().ErrorMessage.Should()
            .Be("The length of Password must be at least 8 characters");
    }

    [Fact(DisplayName = "Should return false when password has no number")]
    public void PasswordIsValid_WhenHasNoNumber_ShouldReturnFalse()
    {
        var plainTextPassword = "Abcd@abcd";

        var password = new Password(plainTextPassword);

        password.IsValid().Should().BeFalse();
        password.ValidationResult?.Errors.First().ErrorMessage.Should()
            .Be("Password must have at least one number");
    }

    [Fact(DisplayName = "Should return false when password has no special character")]
    public void PasswordIsValid_WhenHasNoSpecialCharacter_ShouldReturnFalse()
    {
        var plainTextPassword = "Abcda123";

        var password = new Password(plainTextPassword);

        password.IsValid().Should().BeFalse();
        password.ValidationResult?.Errors.First().ErrorMessage.Should()
            .Be("Password must have at least one special character: !@#$&*");
    }
}
