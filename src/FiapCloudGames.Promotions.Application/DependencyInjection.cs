using FiapCloudGames.Promotions.Application.Features.Calculate;
using FiapCloudGames.Promotions.Application.Features.Campaigns.Create;
using FiapCloudGames.Promotions.Application.Features.Campaigns.Deactivate;
using FiapCloudGames.Promotions.Application.Features.Campaigns.Get;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Promotions.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPromotionsApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateCampaignService>();
        services.AddScoped<GetCampaignService>();
        services.AddScoped<DeactivateCampaignService>();
        services.AddScoped<CalculatePromotionService>();
        return services;
    }
}
