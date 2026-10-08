using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns.Deactivate;

public sealed class DeactivateCampaignService(
    ICampaignRepository campaigns,
    IPromotionsUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await campaigns.FindByIdAsync(campaignId, cancellationToken)
            ?? throw AppException.NotFound("Campanha não encontrada.");

        campaign.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
