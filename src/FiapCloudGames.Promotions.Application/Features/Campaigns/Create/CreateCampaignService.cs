using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Campaigns;
using FiapCloudGames.Promotions.Domain.Common;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns.Create;

public sealed class CreateCampaignService(
    ICampaignRepository campaigns,
    IPromotionsUnitOfWork unitOfWork,
    TimeProvider clock,
    ILogger<CreateCampaignService> logger)
{
    public async Task<CampaignResult> ExecuteAsync(
        CreateCampaignInput input,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);
        logger.LogInformation("Iniciando criação de campanha promocional.");

        Campaign campaign;

        try
        {
            campaign = Campaign.Create(
                input.Name,
                input.DiscountType,
                input.DiscountValue,
                input.StartDateUtc,
                input.EndDateUtc,
                input.GameId,
                clock.GetUtcNow());
        }
        catch (DomainRuleViolationException exception)
        {
            throw AppException.Validation(exception.Message);
        }

        await campaigns.AddAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Campanha {CampaignId} criada com sucesso.",
            campaign.Id);
        return PromotionsApplicationMappings.ToResult(campaign);
    }
}
