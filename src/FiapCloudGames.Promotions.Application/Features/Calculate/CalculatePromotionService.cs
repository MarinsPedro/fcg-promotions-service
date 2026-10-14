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

        if (input.BasePrice > 9999999999.99m)
        {
            throw AppException.Validation(
                [new AppError("O preço-base não pode ser maior que 9999999999.99.", "basePrice")]);
        }

        var basePrice = decimal.Round(input.BasePrice, 2, MidpointRounding.ToEven);
        var nowUtc = DateTimeOffset.UtcNow;

        var eligible = await campaigns.FindEligibleAsync(
            input.GameId,
            nowUtc,
            cancellationToken);

        if (eligible.Count == 0)
        {
            return new CalculatePromotionResult(FinalPrice: basePrice);
        }

        var bestDiscount = eligible[0].CalculateDiscount(basePrice);

        for (var i = 1; i < eligible.Count; i++)
        {
            var discount = eligible[i].CalculateDiscount(basePrice);
            if (discount > bestDiscount)
            {
                bestDiscount = discount;
            }
        }

        var finalPrice = Math.Max(0m, basePrice - bestDiscount);

        return new CalculatePromotionResult(FinalPrice: finalPrice);
    }
}
