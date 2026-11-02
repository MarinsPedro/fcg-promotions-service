using FiapCloudGames.BuildingBlocks.Authentication;
using FiapCloudGames.Promotions.Application;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Campaigns;
using FiapCloudGames.Promotions.Infrastructure.Health;
using FiapCloudGames.Promotions.Infrastructure.Persistence;
using FiapCloudGames.Promotions.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Promotions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPromotionsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "A connection string 'Database' não foi configurada.");

        services.AddPromotionsApplication();
        services.AddFiapServiceAuthentication(configuration);
        services.AddDbContext<PromotionsDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IPromotionsUnitOfWork>(provider =>
            provider.GetRequiredService<PromotionsDbContext>());
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddHealthChecks()
            .AddCheck<PromotionsDatabaseHealthCheck>(
                "promotions-database",
                tags: ["ready"]);

        return services;
    }
}
