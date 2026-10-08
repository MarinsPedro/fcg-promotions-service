using FiapCloudGames.Promotions.Application.Abstractions.Persistence;
using FiapCloudGames.Promotions.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Promotions.Infrastructure.Persistence;

public sealed class PromotionsDbContext(DbContextOptions<PromotionsDbContext> options)
    : DbContext(options), IPromotionsUnitOfWork
{
    public DbSet<Campaign> Campaigns => Set<Campaign>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("promotions");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromotionsDbContext).Assembly);
    }
}
