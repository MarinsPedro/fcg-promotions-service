using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns.Create;

public sealed record CreateCampaignInput(
    string Name,
    DiscountType DiscountType,
    decimal DiscountValue,
    DateTimeOffset StartDateUtc,
    DateTimeOffset EndDateUtc,
    Guid? GameId);
