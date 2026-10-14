using FiapCloudGames.Promotions.Application.Features.Calculate;

namespace FiapCloudGames.Promotions.Presentation.Features.Calculate;

public sealed record CalculatePromotionResponse(
    decimal FinalPrice)
{
    public static CalculatePromotionResponse FromResult(CalculatePromotionResult result) =>
        new(result.FinalPrice);
}
