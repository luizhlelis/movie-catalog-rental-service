using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentMovie.Infrastructure;
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
                // remove the existing context configuration
                var descriptor = services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<RentMovieContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<RentMovieContext>(options =>
                    options.UseInMemoryDatabase("RentMovie"));
            })
        );

        TestServiceScope = server.Host.Services.CreateScope();
        DbContext = TestServiceScope.ServiceProvider.GetRequiredService<RentMovieContext>();

        Client = server.CreateClient();

        if (DbContext.Directors.Any() || !DbContext.Actors.Any())
            return;

        DbContext.Directors.AddRange(Fakers.GetValidDirectors());
        DbContext.Actors.AddRange(Fakers.GetValidCast()
            .Where(actor => actor.Name != "Leonardo Dicaprio").ToList());
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        Client.DefaultRequestHeaders.Remove("Authorization");
        // DbContext.Directors.Clear();
        // DbContext.Actors.Clear();
        DbContext.SaveChanges();
    }
}

public static class EntityExtensions
{
    public static void Clear<T>(this DbSet<T> dbSet) where T : class
    {
        dbSet.RemoveRange(dbSet);
    }
}
