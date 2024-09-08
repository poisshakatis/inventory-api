using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;

namespace App.Contracts.DAL.Repositories;

public interface IStorageRepository : IEntityRepository<Domain.Storage>
{
    Task<IEnumerable<DALDTO.Storage>> AllAsync(Guid userId);
    Task<DALDTO.Storage?> FindWithParentAsync(Guid id);
    void Update(DALDTO.Storage storage, Guid userId);
    void Add(DALDTO.Storage storage, Guid userId);
}