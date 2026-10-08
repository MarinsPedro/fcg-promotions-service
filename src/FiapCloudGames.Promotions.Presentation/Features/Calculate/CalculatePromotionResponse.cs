using FiapCloudGames.Promotions.Application.Features.Calculate;

namespace FiapCloudGames.Promotions.Presentation.Features.Calculate;

public sealed record CalculatePromotionResponse(
    Guid GameId,
    decimal BasePrice,
    decimal Discount,
    decimal FinalPrice,
    string Currency,
    Guid? AppliedCampaignId,
    string? AppliedCampaignName)
{
    public static CalculatePromotionResponse FromResult(CalculatePromotionResult result) =>
        new(
            result.GameId,
            result.BasePrice,
            result.Discount,
            result.FinalPrice,
            result.Currency,
            result.AppliedCampaignId,
            result.AppliedCampaignName);
}
