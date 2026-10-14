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

        var campaign = await campaigns.FindByIdAsync(input.PromotionId, cancellationToken)
            ?? throw AppException.NotFound(
                $"Promoção '{input.PromotionId}' não encontrada.");

        if (!campaign.IsActive)
        {
            throw AppException.BusinessRule("A promoção informada está inativa.");
        }

        var nowUtc = DateTimeOffset.UtcNow;

        if (nowUtc < campaign.StartDateUtc || nowUtc > campaign.EndDateUtc)
        {
            throw AppException.BusinessRule("A promoção informada está fora do período de vigência.");
        }

        if (campaign.GameId.HasValue && campaign.GameId.Value != input.GameId)
        {
            throw AppException.BusinessRule("A promoção informada não é elegível para este jogo.");
        }

        var discount = campaign.CalculateDiscount(basePrice);
        var finalPrice = Math.Max(0m, basePrice - discount);

        return new CalculatePromotionResult(FinalPrice: finalPrice);
    }
}
