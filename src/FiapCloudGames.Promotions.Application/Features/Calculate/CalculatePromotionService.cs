using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Calculate;

public sealed class CalculatePromotionService(ICampaignRepository campaigns)
{
    public async Task<CalculatePromotionResult> ExecuteAsync(
        CalculatePromotionInput input,
        CancellationToken cancellationToken)
    {
        if (input.BasePrice < 0)
        {
            throw AppException.Validation(
                [new AppError("O preço-base não pode ser negativo.", "basePrice")]);
        }

        var eligible = await campaigns.FindEligibleAsync(
            input.GameId,
            input.CalculatedAt,
            cancellationToken);

        if (eligible.Count == 0)
        {
            return new CalculatePromotionResult(
                input.GameId,
                input.BasePrice,
                Discount: 0m,
                FinalPrice: input.BasePrice,
                input.Currency,
                AppliedCampaignId: null,
                AppliedCampaignName: null);
        }

        var bestCampaign = eligible[0];
        var bestDiscount = bestCampaign.CalculateDiscount(input.BasePrice);

        for (var i = 1; i < eligible.Count; i++)
        {
            var discount = eligible[i].CalculateDiscount(input.BasePrice);
            if (discount > bestDiscount)
            {
                bestCampaign = eligible[i];
                bestDiscount = discount;
            }
        }

        var finalPrice = Math.Max(0m, input.BasePrice - bestDiscount);

        return new CalculatePromotionResult(
            input.GameId,
            input.BasePrice,
            Discount: bestDiscount,
            FinalPrice: finalPrice,
            input.Currency,
            AppliedCampaignId: bestCampaign.Id,
            AppliedCampaignName: bestCampaign.Name);
    }
}
