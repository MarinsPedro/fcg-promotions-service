using System.Net;
using FiapCloudGames.Promotions.IntegrationTests.Support;

namespace FiapCloudGames.Promotions.IntegrationTests.Contracts;

public sealed class OpenApiContractTests(PromotionsApiFactory factory)
    : IClassFixture<PromotionsApiFactory>
{
    [Fact]
    public async Task OpenApiSpec_IsAvailable()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("promotions", content, StringComparison.OrdinalIgnoreCase);
    }
}
