using System.Net;
using FiapCloudGames.Promotions.IntegrationTests.Support;

namespace FiapCloudGames.Promotions.IntegrationTests.Contracts;

public sealed class CampaignEndpointTests(PromotionsApiFactory factory)
    : IClassFixture<PromotionsApiFactory>
{
    [Fact]
    public async Task CreateCampaign_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(
            "/internal/v1/campaigns",
            new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCampaign_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/internal/v1/campaigns/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CalculatePromotion_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(
            "/internal/promotions/calculate",
            new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
