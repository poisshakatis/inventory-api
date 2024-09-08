using Base.Contracts.DAL;
using Base.Contracts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL.EF;

public class BaseEntityRepository<TEntity, TDbContext> :
    BaseEntityRepository<Guid, TEntity, TDbContext>, IEntityRepository<TEntity>
    where TEntity : class, IDomainEntityId
    where TDbContext : DbContext
{
    public BaseEntityRepository(TDbContext dbContext) : base(dbContext)
    {
    }
}

public class BaseEntityRepository<TKey, TEntity, TDbContext>
    where TKey : IEquatable<TKey>
    where TEntity : class, IDomainEntityId
    where TDbContext : DbContext

{
    protected readonly DbSet<TEntity> RepoDbSet;

    protected BaseEntityRepository(TDbContext dbContext)
    {
        var repoDbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        RepoDbSet = repoDbContext.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> AllAsync()
    {
        return await RepoDbSet.ToListAsync();
    }

    public virtual async Task<TEntity?> FindAsync(TKey id)
    {
        return await RepoDbSet.FindAsync(id);
    }

    public virtual TEntity Add(TEntity entity)
    {
        return RepoDbSet.Add(entity).Entity;
    }

    public virtual TEntity Update(TEntity entity)
    {
        return RepoDbSet.Update(entity).Entity;
    }

    public virtual TEntity Remove(TEntity entity)
    {
        return RepoDbSet.Remove(entity).Entity;
    }

    public virtual async Task<TEntity?> RemoveAsync(TKey id)
    {
        var entity = await FindAsync(id);
        return entity != null ? Remove(entity) : null;
    }
}