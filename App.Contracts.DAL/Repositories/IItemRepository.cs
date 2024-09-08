using App.Domain.Identity;
using App.DTO.v1_0;
using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;
using Microsoft.AspNetCore.Identity;

namespace App.Contracts.DAL.Repositories;

public interface IItemRepository : IEntityRepository<Domain.Item>
{
    Task<IEnumerable<DALDTO.Item>> AllWithStorageAsync(Guid userId);
    Task<DALDTO.Item?> FindWithStorageAsync(Guid id);
    void Update(DALDTO.Item item);
    void Add(DALDTO.Item item);
    List<UserItemCount> AllUsersWithItemCount(List<AppUser> users);
}