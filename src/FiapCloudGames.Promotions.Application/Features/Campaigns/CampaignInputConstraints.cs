using FiapCloudGames.Promotions.Domain.Campaigns;

namespace FiapCloudGames.Promotions.Application.Features.Campaigns;

public static class CampaignInputConstraints
{
    public const int NameMinimumLength = Campaign.NameMinimumLength;
    public const int NameMaximumLength = Campaign.NameMaximumLength;
    public const decimal MaximumPercentageDiscount = Campaign.MaximumPercentageDiscount;
    public const decimal MaximumMonetaryValue = Campaign.MaximumMonetaryValue;
    public const string MaximumMonetaryValueText = "9999999999.99";
}
