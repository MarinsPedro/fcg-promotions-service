using System.Net;
using FiapCloudGames.Promotions.IntegrationTests.Support;

namespace FiapCloudGames.Promotions.IntegrationTests.Host;

public sealed class HealthEndpointTests(PromotionsApiFactory factory)
    : IClassFixture<PromotionsApiFactory>
{
    [Fact]
    public async Task HealthEndpoints_WhenApplicationIsHealthy_ReportLiveAndReady()
    {
        using var client = factory.CreateClient();

        using var live = await client.GetAsync("/health/live");
        using var ready = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
    }
}
