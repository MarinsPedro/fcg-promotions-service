using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns.Create;

public sealed class CreateCampaignService(
    ICampaignRepository campaigns,
    IPromotionsUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task<CampaignResult> ExecuteAsync(
        CreateCampaignInput input,
        CancellationToken cancellationToken)
    {
        var campaign = Campaign.Create(
            input.Name,
            input.DiscountType,
            input.DiscountValue,
            input.StartDateUtc,
            input.EndDateUtc,
            input.GameId,
            clock.GetUtcNow());

        await campaigns.AddAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PromotionsApplicationMappings.ToResult(campaign);
    }
}
