using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Promotions.Application.Features.Calculate;
using FiapCloudGames.Promotions.Application.Features.Campaigns;
using FiapCloudGames.Promotions.Presentation.Validation;

namespace FiapCloudGames.Promotions.Presentation.Features.Calculate;

public sealed record CalculatePromotionRequest(
    [NotEmptyGuid] Guid GameId,
    [Range(
        typeof(decimal),
        "0",
        CampaignInputConstraints.MaximumMonetaryValueText,
        ParseLimitsInInvariantCulture = true)]
    decimal BasePrice,
    [NotEmptyGuid] Guid PromotionId)
{
    public CalculatePromotionInput ToInput() =>
        new(GameId, BasePrice, PromotionId);
}
