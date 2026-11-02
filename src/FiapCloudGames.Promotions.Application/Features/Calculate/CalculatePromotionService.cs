using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Promotions.Domain.Campaigns;
using FiapCloudGames.Promotions.Domain.Common;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Promotions.Application.Features.Calculate;

public sealed class CalculatePromotionService(
    ICampaignRepository campaigns,
    TimeProvider clock,
    ILogger<CalculatePromotionService> logger)
{
    public async Task<CalculatePromotionResult> ExecuteAsync(
        CalculatePromotionInput input,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);
        logger.LogDebug(
            "Calculando promoção {PromotionId} para o jogo {GameId}.",
            input.PromotionId,
            input.GameId);

        var basePrice = decimal.Round(input.BasePrice, 2, MidpointRounding.ToEven);

        var campaign = await campaigns.FindByIdAsync(input.PromotionId, cancellationToken)
            ?? throw AppException.NotFound(
                $"Promoção '{input.PromotionId}' não encontrada.");

        decimal discount;

        try
        {
            discount = campaign.CalculateDiscount(basePrice);
        }
        catch (DomainRuleViolationException exception)
        {
            throw AppException.Validation(exception.Message);
        }

        var nowUtc = clock.GetUtcNow();

        switch (campaign.GetEligibility(input.GameId, nowUtc))
        {
            case CampaignEligibility.Inactive:
                throw AppException.BusinessRule("A promoção informada está inativa.");
            case CampaignEligibility.OutsideValidityPeriod:
                throw AppException.BusinessRule(
                    "A promoção informada está fora do período de vigência.");
            case CampaignEligibility.GameNotEligible:
                throw AppException.BusinessRule(
                    "A promoção informada não é elegível para este jogo.");
        }

        var finalPrice = Math.Max(0m, basePrice - discount);

        logger.LogDebug(
            "Promoção {PromotionId} calculada para o jogo {GameId}.",
            campaign.Id,
            input.GameId);

        return new CalculatePromotionResult(FinalPrice: finalPrice);
    }
}
