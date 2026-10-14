using FiapCloudGames.Promotions.Domain.Campaigns;
using FiapCloudGames.Promotions.Domain.Common;

namespace FiapCloudGames.Promotions.UnitTests.Domain.Campaigns;

public sealed class CampaignTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var campaign = Campaign.Create(
            "Black Friday",
            DiscountType.Percentage,
            25m,
            Now,
            Now.AddDays(7),
            gameId: null,
            Now);

        Assert.Equal("Black Friday", campaign.Name);
        Assert.Equal(DiscountType.Percentage, campaign.DiscountType);
        Assert.Equal(25m, campaign.DiscountValue);
        Assert.True(campaign.IsActive);
        Assert.Null(campaign.GameId);
    }

    [Fact]
    public void Create_WithPercentageAbove100_Throws()
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            Campaign.Create(
                "Invalid",
                DiscountType.Percentage,
                101m,
                Now,
                Now.AddDays(1),
                null,
                Now));
    }

    [Fact]
    public void CalculateDiscount_Percentage_ReturnsCorrectValue()
    {
        var campaign = Campaign.Create(
            "20% Off",
            DiscountType.Percentage,
            20m,
            Now,
            Now.AddDays(7),
            null,
            Now);

        var discount = campaign.CalculateDiscount(100m);

        Assert.Equal(20m, discount);
    }

    [Fact]
    public void CalculateDiscount_Fixed_CapsAtBasePrice()
    {
        var campaign = Campaign.Create(
            "R$50 Off",
            DiscountType.Fixed,
            50m,
            Now,
            Now.AddDays(7),
            null,
            Now);

        var discount = campaign.CalculateDiscount(30m);

        Assert.Equal(30m, discount);
    }

    [Fact]
    public void IsEligible_ActiveAndInPeriod_ReturnsTrue()
    {
        var gameId = Guid.NewGuid();
        var campaign = Campaign.Create(
            "Promo",
            DiscountType.Fixed,
            10m,
            Now,
            Now.AddDays(7),
            gameId,
            Now);

        Assert.True(campaign.IsEligible(gameId, Now.AddDays(1)));
    }

    [Fact]
    public void IsEligible_DifferentGame_ReturnsFalse()
    {
        var campaign = Campaign.Create(
            "Promo",
            DiscountType.Fixed,
            10m,
            Now,
            Now.AddDays(7),
            Guid.NewGuid(),
            Now);

        Assert.False(campaign.IsEligible(Guid.NewGuid(), Now.AddDays(1)));
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var campaign = Campaign.Create(
            "Promo",
            DiscountType.Fixed,
            10m,
            Now,
            Now.AddDays(7),
            null,
            Now);

        campaign.Deactivate();

        Assert.False(campaign.IsActive);
    }

    [Fact]
    public void Create_WithFixedDiscountAboveMax_Throws()
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            Campaign.Create(
                "Invalid",
                DiscountType.Fixed,
                10000000000m,
                Now,
                Now.AddDays(1),
                null,
                Now));
    }

    [Fact]
    public void Create_WithFixedDiscountAtMax_Succeeds()
    {
        var campaign = Campaign.Create(
            "Max Value",
            DiscountType.Fixed,
            9999999999.99m,
            Now,
            Now.AddDays(7),
            null,
            Now);

        Assert.Equal(9999999999.99m, campaign.DiscountValue);
    }

    [Fact]
    public void Create_RoundsDiscountValueToTwoDecimals()
    {
        var campaign = Campaign.Create(
            "Rounded",
            DiscountType.Fixed,
            15.555m,
            Now,
            Now.AddDays(7),
            null,
            Now);

        Assert.Equal(15.56m, campaign.DiscountValue);
    }

    [Fact]
    public void Create_WithNegativeDiscount_Throws()
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            Campaign.Create(
                "Invalid",
                DiscountType.Fixed,
                -5m,
                Now,
                Now.AddDays(1),
                null,
                Now));
    }

    [Fact]
    public void Create_WithZeroDiscount_Throws()
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            Campaign.Create(
                "Invalid",
                DiscountType.Fixed,
                0m,
                Now,
                Now.AddDays(1),
                null,
                Now));
    }
}
