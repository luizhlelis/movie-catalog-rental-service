using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using RentMovie.Test.Helpers;
using Xunit;

namespace RentMovie.Test.Integration.Controllers;

public class AuthenticationControllerTest : IntegrationTestFixture
{
    private const string PostTokenPath = "v1/authentication/token";

    [Fact(DisplayName = "Should return forbidden when there is no user with the incoming username")]
    public async Task PostToken_WhenUserNotFound_ShouldReturnForbidden()
    {
        // arrange
        var requestBody = new {Username = "user-not-found", Password = "NotFound@123"};
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        response.Should().Be403Forbidden();
    }
}
