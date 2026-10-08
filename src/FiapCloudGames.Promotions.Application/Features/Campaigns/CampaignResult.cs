using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns;

public sealed record CampaignResult(
    Guid Id,
    string Name,
    DiscountType DiscountType,
    decimal DiscountValue,
    DateTimeOffset StartDateUtc,
    DateTimeOffset EndDateUtc,
    Guid? GameId,
    bool IsActive,
    DateTimeOffset CreatedAtUtc);
