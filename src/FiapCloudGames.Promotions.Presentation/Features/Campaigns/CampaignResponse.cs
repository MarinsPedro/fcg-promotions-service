using FiapCloudGames.Promotions.Application.Features.Campaigns;

namespace FiapCloudGames.Promotions.Presentation.Features.Campaigns;

public sealed record CampaignResponse(
    Guid Id,
    string Name,
    string DiscountType,
    decimal DiscountValue,
    DateTimeOffset StartDateUtc,
    DateTimeOffset EndDateUtc,
    Guid? GameId,
    bool IsActive,
    DateTimeOffset CreatedAtUtc)
{
    public static CampaignResponse FromResult(CampaignResult result) =>
        new(
            result.Id,
            result.Name,
            result.DiscountType.ToString(),
            result.DiscountValue,
            result.StartDateUtc,
            result.EndDateUtc,
            result.GameId,
            result.IsActive,
            result.CreatedAtUtc);
}
