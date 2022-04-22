using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.ValueObjects;
using RentMovie.Test.Helpers;
using RentMovie.Web.Responses;
using Xunit;

namespace RentMovie.Test.Integration.Controllers;

public class AuthenticationControllerTest : IntegrationTestFixture
{
    private const string PostTokenPath = "v1/authentication/token";
    private readonly Password _validPassword;

    public AuthenticationControllerTest()
    {
        _validPassword = new Password("StrongPassword@123");
    }

    [Fact(DisplayName = "Should return ok with token when valid owner credential")]
    public async Task PostToken_WhenValidOwnerCredential_ShouldReturnOkWithToken()
    {
        var user = new User("valid-username-1", _validPassword);
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        // arrange
        var requestBody = new {user.Username, Password = _validPassword.PlainText};
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be200Ok();
    }

    [Theory(DisplayName = "Should return bad request when empty or null username")]
    [InlineData(null)]
    [InlineData("")]
    public async Task PostToken_WhenEmptyUsername_ShouldReturnBadRequest(string username)
    {
        // arrange
        var requestBody = new {Username = username, Password = "ValidPwd@123"};
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be400BadRequest();
    }

    [Theory(DisplayName = "Should return bad request when empty or null password")]
    [InlineData(null)]
    [InlineData("")]
    public async Task PostToken_WhenEmptyPassword_ShouldReturnBadRequest(string password)
    {
        // arrange
        var requestBody = new {Username = "valid-username", Password = password};
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be400BadRequest();
    }

    [Fact(DisplayName = "Should return forbidden when there is no user with the incoming username")]
    public async Task PostToken_WhenUserNotFound_ShouldReturnForbidden()
    {
        // arrange
        var requestBody = new {Username = "user-forbidden", Password = "Forbidden@123"};
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");
        var expectedResponse = new ForbiddenResponse("User or password mismatch", string.Empty);

        // act
        var response = await Client.PostAsync(PostTokenPath, content);
        var responseMessage = await response.Content.ReadAsStringAsync();
        var responseBody = JsonConvert.DeserializeObject<ForbiddenResponse>(responseMessage);

        // assert
        response.Should().Be403Forbidden();
        responseBody.Should().BeEquivalentTo(expectedResponse,
            options => options.Excluding(source => source.TraceId)
        );
    }

    [Fact(DisplayName = "Should return forbidden when password mismatch")]
    public async Task PostToken_WhenPasswordMismatch_ShouldReturnForbidden()
    {
        var user = new User("valid-username-4", _validPassword);
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        // arrange
        var requestBody = new {user.Username, Password = "Fake@123"};
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");
        var expectedResponse = new ForbiddenResponse("User or password mismatch", string.Empty);

        // act
        var response = await Client.PostAsync(PostTokenPath, content);
        var responseMessage = await response.Content.ReadAsStringAsync();
        var responseBody = JsonConvert.DeserializeObject<ForbiddenResponse>(responseMessage);

        // assert
        response.Should().Be403Forbidden();
        responseBody.Should().BeEquivalentTo(expectedResponse,
            options => options.Excluding(source => source.TraceId)
        );
    }
}
