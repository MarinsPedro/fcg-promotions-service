namespace FiapCloudGames.Promotions.Application.Features.Calculate;

public sealed record CalculatePromotionResult(
    Guid GameId,
    decimal BasePrice,
    decimal Discount,
    decimal FinalPrice,
    string Currency,
    Guid? AppliedCampaignId,
    string? AppliedCampaignName);
