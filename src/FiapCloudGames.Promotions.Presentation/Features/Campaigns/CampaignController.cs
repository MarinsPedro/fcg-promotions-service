using FiapCloudGames.Promotions.Application.Features.Campaigns.Create;
using FiapCloudGames.Promotions.Application.Features.Campaigns.Deactivate;
using FiapCloudGames.Promotions.Application.Features.Campaigns.Get;
using FiapCloudGames.Promotions.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Promotions.Presentation.Features.Campaigns;

[ApiController]
[Route("internal/v1/campaigns")]
public sealed class CampaignController : ControllerBase
{
    private const string GetByIdRoute = "Promotions.Campaigns.GetById";

    [HttpPost]
    [Authorize(Policy = PromotionsPolicies.CampaignsWrite)]
    [ProducesResponseType<CampaignResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CampaignResponse>> CreateAsync(
        [FromBody] CreateCampaignRequest request,
        [FromServices] CreateCampaignService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(request.ToInput(), cancellationToken);
        var response = CampaignResponse.FromResult(result);
        return CreatedAtRoute(GetByIdRoute, new { campaignId = response.Id }, response);
    }

    [HttpGet("{campaignId:guid}", Name = GetByIdRoute)]
    [Authorize(Policy = PromotionsPolicies.CampaignsRead)]
    [ProducesResponseType<CampaignResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CampaignResponse>> GetAsync(
        Guid campaignId,
        [FromServices] GetCampaignService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(campaignId, cancellationToken);
        return Ok(CampaignResponse.FromResult(result));
    }

    [HttpDelete("{campaignId:guid}")]
    [Authorize(Policy = PromotionsPolicies.CampaignsWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeactivateAsync(
        Guid campaignId,
        [FromServices] DeactivateCampaignService service,
        CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(campaignId, cancellationToken);
        return NoContent();
    }
}
