namespace FiapCloudGames.Promotions.Domain.Common;

public sealed class DomainRuleViolationException(string message)
    : Exception(message);
