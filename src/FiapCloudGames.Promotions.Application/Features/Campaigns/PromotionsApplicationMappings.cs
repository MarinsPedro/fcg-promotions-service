using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns;

internal static class PromotionsApplicationMappings
{
    public static CampaignResult ToResult(Campaign campaign) =>
        new(
            campaign.Id,
            campaign.Name,
            campaign.DiscountType,
            campaign.DiscountValue,
            campaign.StartDateUtc,
            campaign.EndDateUtc,
            campaign.GameId,
            campaign.IsActive,
            campaign.CreatedAtUtc);
}
