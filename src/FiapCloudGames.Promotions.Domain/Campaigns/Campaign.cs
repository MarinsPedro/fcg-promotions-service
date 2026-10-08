using FiapCloudGames.Promotions.Domain.Common;

namespace FiapCloudGames.Promotions.Domain.Campaigns;

public sealed class Campaign
{
    private Campaign() { }

    private Campaign(
        Guid id,
        string name,
        DiscountType discountType,
        decimal discountValue,
        DateTimeOffset startDateUtc,
        DateTimeOffset endDateUtc,
        Guid? gameId,
        DateTimeOffset createdAtUtc)
    {
        Id = id != Guid.Empty
            ? id
            : throw new DomainRuleViolationException("O identificador da campanha é obrigatório.");
        ChangeName(name);
        DiscountType = discountType is not DiscountType.Undefined && Enum.IsDefined(discountType)
            ? discountType
            : throw new DomainRuleViolationException("O tipo de desconto é inválido.");
        DiscountValue = discountValue > 0
            ? discountValue
            : throw new DomainRuleViolationException("O valor do desconto deve ser maior que zero.");
        if (discountType == DiscountType.Percentage && discountValue > 100)
        {
            throw new DomainRuleViolationException("O percentual de desconto não pode exceder 100.");
        }
        ValidatePeriod(startDateUtc, endDateUtc);
        StartDateUtc = startDateUtc;
        EndDateUtc = endDateUtc;
        GameId = gameId;
        IsActive = true;
        CreatedAtUtc = createdAtUtc != default && createdAtUtc.Offset == TimeSpan.Zero
            ? createdAtUtc
            : throw new DomainRuleViolationException("A data de criação deve estar em UTC.");
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public DateTimeOffset StartDateUtc { get; private set; }
    public DateTimeOffset EndDateUtc { get; private set; }
    public Guid? GameId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static Campaign Create(
        string name,
        DiscountType discountType,
        decimal discountValue,
        DateTimeOffset startDateUtc,
        DateTimeOffset endDateUtc,
        Guid? gameId,
        DateTimeOffset createdAtUtc) =>
        new(Guid.NewGuid(), name, discountType, discountValue, startDateUtc, endDateUtc, gameId, createdAtUtc);

    public bool IsEligible(Guid gameId, DateTimeOffset nowUtc) =>
        IsActive &&
        nowUtc >= StartDateUtc &&
        nowUtc <= EndDateUtc &&
        (!GameId.HasValue || GameId.Value == gameId);

    public decimal CalculateDiscount(decimal basePrice) =>
        DiscountType switch
        {
            DiscountType.Fixed => Math.Min(DiscountValue, basePrice),
            DiscountType.Percentage => Math.Round(basePrice * DiscountValue / 100m, 2, MidpointRounding.ToEven),
            _ => 0m
        };

    public void ChangeName(string name)
    {
        Name = name?.Trim() is { Length: >= 3 and <= 200 } normalized
            ? normalized
            : throw new DomainRuleViolationException("O nome da campanha deve ter entre 3 e 200 caracteres.");
    }

    public void Deactivate() => IsActive = false;

    private static void ValidatePeriod(DateTimeOffset start, DateTimeOffset end)
    {
        if (start.Offset != TimeSpan.Zero || end.Offset != TimeSpan.Zero)
        {
            throw new DomainRuleViolationException("As datas de vigência devem estar em UTC.");
        }

        if (end <= start)
        {
            throw new DomainRuleViolationException("A data de término deve ser posterior à data de início.");
        }
    }
}
