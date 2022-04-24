using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Domain.ValueObjects;
using RentMovie.Test.Helpers;
using Xunit;

namespace RentMovie.Test.Integration.Controllers;

public class MovieControllerTest : IntegrationTestFixture
{
    private const string MoviePath = "v1/movie";

    public MovieControllerTest()
    {
        var auth = TestServiceScope.ServiceProvider.GetRequiredService<Authentication>();
        var user = new User("movie-test-user-admin", "StrongPassword@123", "12345", Role.Admin);
        var registeredUser =
            DbContext.Users.FirstOrDefault(dbUser => dbUser.Username == user.Username);

        var accessToken = auth.GenerateAccessToken(user.Username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        if (registeredUser is not null)
            return;

        DbContext.Users.Add(user);
        DbContext.SaveChanges();
    }

    [Fact(DisplayName = "Should return created when movie doesn't exist yet")]
    public async Task PostMovie_WhenMovieDoesNotExist_ShouldReturnCreated()
    {
        // arrange
        var movie = Fakers.GetValidMovie();
        var content = new StringContent(
            JsonConvert.SerializeObject(movie),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(MoviePath, content);

        // assert
        response.Should().Be201Created().And.BeAs(movie,
            options => options.Excluding(source => source.Id));
    }

    [Fact(DisplayName = "Should return bad request when movie already exists")]
    public async Task PostMovie_WhenMovieAlreadyExists_ShouldReturnBadRequest()
    {
        // arrange
        var movie = Fakers.GetValidMovie();
        var content = new StringContent(
            JsonConvert.SerializeObject(movie),
            Encoding.UTF8,
            "application/json");
        await Client.PostAsync(MoviePath, content);

        // act
        var response = await Client.PostAsync(MoviePath, content);

        // assert
        response.Should()
            .Be400BadRequest().And
            .BeAs(new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    traceId = "0HMH5DLVSLJDP",
                    errors = new
                    {
                        AsyncPredicateValidator = new[]
                        {
                            "Movie has already been registered"
                        }
                    }
                },
                options => options.Excluding(source => source.traceId));
    }
}
