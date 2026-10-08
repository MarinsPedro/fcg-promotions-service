using FiapCloudGames.Promotions.Application.Features.Campaigns.Create;
using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Presentation.Features.Campaigns;

public sealed record CreateCampaignRequest(
    string Name,
    DiscountType DiscountType,
    decimal DiscountValue,
    DateTimeOffset StartDateUtc,
    DateTimeOffset EndDateUtc,
    Guid? GameId)
{
    public CreateCampaignInput ToInput() =>
        new(Name, DiscountType, DiscountValue, StartDateUtc, EndDateUtc, GameId);
}
