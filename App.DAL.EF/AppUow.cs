using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.EF.Repositories;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUow : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
{
    public AppUow(AppDbContext dbContext) : base(dbContext)
    {
    }

    private IStorageRepository? _storages;
    private IItemRepository? _items;
    public IStorageRepository Storages => _storages ?? new StorageRepository(UowDbContext);
    public IItemRepository Items => _items ?? new ItemRepository(UowDbContext);
}