using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentMovie.Infrastructure.Adapters;
using RentMovie.Web;

namespace RentMovie.Test.Helpers;

public class IntegrationTestFixture : IDisposable
{
    protected HttpClient Client;
    protected RentMovieContext DbContext;
    protected IServiceScope TestServiceScope;

    public IntegrationTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, false)
            .AddEnvironmentVariables()
            .Build();

        var server = new TestServer(new WebHostBuilder()
            .UseConfiguration(configuration)
            .UseStartup<Startup>()
            .ConfigureTestServices(services =>
            {
                services.AddSingleton<IConfiguration>(configuration);
                services.AddDbContext<RentMovieContext>(options =>
                    options.UseInMemoryDatabase("RentMovie"));
            })
        );

        TestServiceScope = server.Host.Services.CreateScope();
        DbContext = TestServiceScope.ServiceProvider.GetRequiredService<RentMovieContext>();
        Client = server.CreateClient();
    }

    public void Dispose()
    {
        Client.DefaultRequestHeaders.Remove("Authorization");
    }
}
