namespace FiapCloudGames.Promotions.Domain.Campaigns;

public enum CampaignEligibility
{
    Eligible = 1,
    Inactive = 2,
    OutsideValidityPeriod = 3,
    GameNotEligible = 4
}
