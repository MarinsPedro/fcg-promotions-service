namespace FiapCloudGames.Promotions.Application.Features.Calculate;

public sealed record CalculatePromotionInput(
    Guid GameId,
    decimal BasePrice,
    string Currency,
    DateTimeOffset CalculatedAt);
