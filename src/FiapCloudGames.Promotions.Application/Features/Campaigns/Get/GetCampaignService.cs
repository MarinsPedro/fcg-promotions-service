using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Promotions.Domain.Campaigns;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns.Get;

public sealed class GetCampaignService(
    ICampaignRepository campaigns,
    ILogger<GetCampaignService> logger)
{
    public async Task<CampaignResult> ExecuteAsync(
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Consultando campanha {CampaignId}.", campaignId);

        var campaign = await campaigns.FindByIdAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            logger.LogDebug(
                "Campanha {CampaignId} não encontrada.",
                campaignId);
            throw AppException.NotFound("Campanha não encontrada.");
        }

        return PromotionsApplicationMappings.ToResult(campaign);
    }
}
