using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns.Get;

public sealed class GetCampaignService(ICampaignRepository campaigns)
{
    public async Task<CampaignResult> ExecuteAsync(
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await campaigns.FindByIdAsync(campaignId, cancellationToken)
            ?? throw AppException.NotFound("Campanha não encontrada.");

        return PromotionsApplicationMappings.ToResult(campaign);
    }
}
