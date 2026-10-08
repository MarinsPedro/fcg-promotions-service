namespace FiapCloudGames.Promotions.Application.Abstractions.Persistence;

public interface IPromotionsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
