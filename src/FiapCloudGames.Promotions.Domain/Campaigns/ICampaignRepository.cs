namespace FiapCloudGames.Promotions.Domain.Campaigns;

public interface ICampaignRepository
{
    Task AddAsync(Campaign campaign, CancellationToken cancellationToken);
    Task<Campaign?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Campaign>> FindEligibleAsync(
        Guid gameId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken);
}
