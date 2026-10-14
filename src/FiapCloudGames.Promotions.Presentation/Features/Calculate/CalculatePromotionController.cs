using FiapCloudGames.Promotions.Application.Features.Calculate;
using FiapCloudGames.Promotions.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Promotions.Presentation.Features.Calculate;

[ApiController]
[Route("internal/v1/promotions")]
public sealed class CalculatePromotionController : ControllerBase
{
    [HttpPost("calculate")]
    [Authorize(Policy = PromotionsPolicies.Calculate)]
    [ProducesResponseType<CalculatePromotionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CalculatePromotionResponse>> PostCalcularDesconto(
        [FromBody] CalculatePromotionRequest request,
        [FromServices] CalculatePromotionService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(request.ToInput(), cancellationToken);
        return Ok(CalculatePromotionResponse.FromResult(result));
    }
}
