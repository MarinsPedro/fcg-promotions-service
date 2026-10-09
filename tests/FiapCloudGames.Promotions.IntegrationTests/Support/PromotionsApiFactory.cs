using FiapCloudGames.Promotions.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.IntegrationTests.Support;

public sealed class PromotionsApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"promotions-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] =
                    "Host=localhost;Database=integration;Username=test;Password=test",
                ["Authentication:Authority"] = "https://localhost",
                ["Authentication:Audience"] = "promotions-api",
                ["Authentication:Issuer"] = "integration-issuer",
                ["Logging:LogLevel:Default"] = "Warning"
            });
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PromotionsDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<PromotionsDbContext>>();
            services.AddDbContext<PromotionsDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
