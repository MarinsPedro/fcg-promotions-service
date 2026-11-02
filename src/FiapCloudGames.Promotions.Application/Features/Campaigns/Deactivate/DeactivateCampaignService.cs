using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Campaigns;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns.Deactivate;

public sealed class DeactivateCampaignService(
    ICampaignRepository campaigns,
    IPromotionsUnitOfWork unitOfWork,
    ILogger<DeactivateCampaignService> logger)
{
    public async Task ExecuteAsync(
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Iniciando desativação da campanha {CampaignId}.",
            campaignId);

        var campaign = await campaigns.FindByIdAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            logger.LogInformation(
                "Não foi possível desativar: campanha {CampaignId} não encontrada.",
                campaignId);
            throw AppException.NotFound("Campanha não encontrada.");
        }

        if (!campaign.IsActive)
        {
            logger.LogInformation(
                "Não foi possível desativar: campanha {CampaignId} já está inativa.",
                campaignId);
            throw AppException.Conflict("Campanha já desativada.");
        }

        campaign.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Campanha {CampaignId} desativada com sucesso.",
            campaign.Id);
    }
}
