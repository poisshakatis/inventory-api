using Base.Contracts.Domain;

namespace Base.Contracts.DAL;

public interface IEntityRepository<TEntity> : IEntityRepository<TEntity, Guid>
    where TEntity : class, IDomainEntityId;

public interface IEntityRepository<TEntity, in TKey>
    where TEntity : class, IDomainEntityId<TKey>
    where TKey : IEquatable<TKey>
{
    Task<IEnumerable<TEntity>> AllAsync();

    Task<TEntity?> FindAsync(TKey id);

    TEntity Add(TEntity entity);

    TEntity Update(TEntity entity);

    TEntity Remove(TEntity entity);

    Task<TEntity?> RemoveAsync(TKey id);
}