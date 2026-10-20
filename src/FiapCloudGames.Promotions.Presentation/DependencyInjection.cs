using FiapCloudGames.BuildingBlocks.Authentication;
using FiapCloudGames.Promotions.Presentation.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Promotions.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPromotionsPresentation(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(DependencyInjection).Assembly);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                PromotionsPolicies.CampaignsRead,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireServiceToken()
                    .RequireScopes("promotions.campaigns.read"));
            options.AddPolicy(
                PromotionsPolicies.CampaignsWrite,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireServiceToken()
                    .RequireScopes("promotions.campaigns.write"));
            options.AddPolicy(
                PromotionsPolicies.Calculate,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireServiceToken()
                    .RequireScopes("promotions.calculate"));
        });

        return services;
    }
}
