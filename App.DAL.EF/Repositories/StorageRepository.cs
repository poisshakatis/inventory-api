using App.Contracts.DAL.Repositories;
using DALDTO = App.DAL.DTO;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class StorageRepository : BaseEntityRepository<Domain.Storage, AppDbContext>,
    IStorageRepository
{
    public StorageRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<DALDTO.Storage>> AllAsync(Guid userId)
    {
        var res = await RepoDbSet
            .Include(s => s.AppUser)
            .Include(s => s.ParentStorage)
            .Where(s => s.AppUserId.Equals(userId))
            .ToListAsync();

        return res.Select(MapDomainToDalDto);
    }

    private static DALDTO.Storage MapDomainToDalDto(Domain.Storage storage)
    {
        return new DALDTO.Storage
        {
            Id = storage.Id,
            Name = storage.Name,
            ParentStorageId = storage.ParentStorageId,
            ParentStorageName = storage.ParentStorage?.Name
        };
    }

    public async Task<DALDTO.Storage?> FindWithParentAsync(Guid id)
    {
        var res = await RepoDbSet
            .Include(s => s.ParentStorage)
            .FirstOrDefaultAsync(s => s.Id.Equals(id));

        return res != null ? MapDomainToDalDto(res) : null;
    }

    public void Update(DALDTO.Storage storage, Guid userId)
    {
        RepoDbSet.Update(MapDalDtoToDomain(storage, userId));
    }

    private static Domain.Storage MapDalDtoToDomain(DALDTO.Storage storage, Guid userId)
    {
        return new Domain.Storage
        {
            Id = storage.Id,
            Name = storage.Name,
            ParentStorageId = storage.ParentStorageId,
            AppUserId = userId
        };
    }

    public void Add(DALDTO.Storage storage, Guid userId)
    {
        RepoDbSet.Add(MapDalDtoToDomain(storage, userId));
    }
}