using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Promotions.Application.Features.Campaigns;
using FiapCloudGames.Promotions.Application.Features.Campaigns.Create;
using FiapCloudGames.Promotions.Domain.Campaigns;
using FiapCloudGames.Promotions.Presentation.Validation;

namespace FiapCloudGames.Promotions.Presentation.Features.Campaigns;

public sealed record CreateCampaignRequest(
    [Required]
    [ApiStringLength(
        CampaignInputConstraints.NameMaximumLength,
        MinimumLength = CampaignInputConstraints.NameMinimumLength)]
    string Name,
    [EnumDataType(typeof(DiscountType))] DiscountType DiscountType,
    [Range(
        typeof(decimal),
        "0.01",
        CampaignInputConstraints.MaximumMonetaryValueText,
        ParseLimitsInInvariantCulture = true)]
    decimal DiscountValue,
    DateTimeOffset StartDateUtc,
    DateTimeOffset EndDateUtc,
    [NotEmptyGuid] Guid? GameId)
{
    public CreateCampaignInput ToInput() =>
        new(Name, DiscountType, DiscountValue, StartDateUtc, EndDateUtc, GameId);
}
