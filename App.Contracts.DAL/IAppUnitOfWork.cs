using App.Contracts.DAL.Repositories;
using Base.Contracts.Domain;

namespace App.Contracts.DAL;

public interface IAppUnitOfWork : IUnitOfWork
{
    IStorageRepository Storages { get; }
    IItemRepository Items { get; }
}