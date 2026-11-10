
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RaceDay.API.Data;

namespace RaceDay.Tests;

public class RaceDayWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        "RaceDayTests_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<RaceDayContext>>();
            services.RemoveAll<RaceDayContext>();

            services.AddDbContext<RaceDayContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
}

