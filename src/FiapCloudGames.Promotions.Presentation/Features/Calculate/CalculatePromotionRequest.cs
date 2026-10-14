using FiapCloudGames.Promotions.Application.Features.Calculate;

namespace FiapCloudGames.Promotions.Presentation.Features.Calculate;

public sealed record CalculatePromotionRequest(
    Guid GameId,
    decimal BasePrice)
{
    public CalculatePromotionInput ToInput() =>
        new(GameId, BasePrice);
}
