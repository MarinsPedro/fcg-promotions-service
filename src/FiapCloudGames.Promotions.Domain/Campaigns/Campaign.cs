using FiapCloudGames.Promotions.Domain.Common;

namespace FiapCloudGames.Promotions.Domain.Campaigns;

public sealed class Campaign
{
    public const int NameMinimumLength = 3;
    public const int NameMaximumLength = 200;
    public const decimal MaximumPercentageDiscount = 100m;
    public const decimal MaximumMonetaryValue = 9999999999.99m;

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
        if (discountValue <= 0)
        {
            throw new DomainRuleViolationException("O valor do desconto deve ser maior que zero.");
        }

        if (discountType == DiscountType.Percentage &&
            discountValue > MaximumPercentageDiscount)
        {
            throw new DomainRuleViolationException("O percentual de desconto não pode exceder 100.");
        }

        if (discountType == DiscountType.Fixed &&
            discountValue > MaximumMonetaryValue)
        {
            throw new DomainRuleViolationException("O valor do desconto fixo não pode ser maior que 9999999999.99.");
        }

        DiscountValue = decimal.Round(discountValue, 2, MidpointRounding.ToEven);
        ValidatePeriod(startDateUtc, endDateUtc);
        StartDateUtc = startDateUtc;
        EndDateUtc = endDateUtc;
        if (gameId == Guid.Empty)
        {
            throw new DomainRuleViolationException(
                "O identificador do jogo deve ser válido quando informado.");
        }

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

    public CampaignEligibility GetEligibility(Guid gameId, DateTimeOffset nowUtc)
    {
        if (!IsActive)
        {
            return CampaignEligibility.Inactive;
        }

        if (nowUtc < StartDateUtc || nowUtc > EndDateUtc)
        {
            return CampaignEligibility.OutsideValidityPeriod;
        }

        if (gameId == Guid.Empty ||
            GameId.HasValue && GameId.Value != gameId)
        {
            return CampaignEligibility.GameNotEligible;
        }

        return CampaignEligibility.Eligible;
    }

    public bool IsEligible(Guid gameId, DateTimeOffset nowUtc) =>
        GetEligibility(gameId, nowUtc) == CampaignEligibility.Eligible;

    public decimal CalculateDiscount(decimal basePrice)
    {
        if (basePrice < 0 || basePrice > MaximumMonetaryValue)
        {
            throw new DomainRuleViolationException(
                "O preço-base deve estar entre 0 e 9999999999.99.");
        }

        return DiscountType switch
        {
            DiscountType.Fixed => Math.Min(DiscountValue, basePrice),
            DiscountType.Percentage => Math.Round(basePrice * DiscountValue / 100m, 2, MidpointRounding.ToEven),
            _ => throw new DomainRuleViolationException(
                "O tipo de desconto da campanha é inválido.")
        };
    }

    public void ChangeName(string name)
    {
        Name = name?.Trim() is
        {
            Length: >= NameMinimumLength and <= NameMaximumLength
        } normalized
            ? normalized
            : throw new DomainRuleViolationException(
                $"O nome da campanha deve ter entre {NameMinimumLength} e " +
                $"{NameMaximumLength} caracteres.");
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
