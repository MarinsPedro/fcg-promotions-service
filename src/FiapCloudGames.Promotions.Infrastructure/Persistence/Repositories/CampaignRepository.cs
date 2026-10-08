using FiapCloudGames.Promotions.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Promotions.Infrastructure.Persistence.Repositories;

internal sealed class CampaignRepository(PromotionsDbContext context) : ICampaignRepository
{
    public Task AddAsync(Campaign campaign, CancellationToken cancellationToken) =>
        context.Campaigns.AddAsync(campaign, cancellationToken).AsTask();

    public Task<Campaign?> FindByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Campaigns.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Campaign>> FindEligibleAsync(
        Guid gameId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken) =>
        await context.Campaigns
            .Where(c => c.IsActive &&
                        c.StartDateUtc <= nowUtc &&
                        c.EndDateUtc >= nowUtc &&
                        (!c.GameId.HasValue || c.GameId.Value == gameId))
            .ToListAsync(cancellationToken);
}
