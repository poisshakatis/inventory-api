using App.Contracts.DAL.Repositories;
using App.Domain.Identity;
using App.DTO.v1_0;
using DALDTO = App.DAL.DTO;
using Base.DAL.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class ItemRepository : BaseEntityRepository<Domain.Item, AppDbContext>,
    IItemRepository
{
    public ItemRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<DALDTO.Item>> AllWithStorageAsync(Guid userId)
    {
        var res = await RepoDbSet
            .Include(i => i.Storage)
            .Where(i => i.Storage!.AppUserId.Equals(userId))
            .ToListAsync();

        return res.Select(MapDomainToDalDto);
    }

    private static DALDTO.Item MapDomainToDalDto(Domain.Item item)
    {
        return new DALDTO.Item
        {
            Id = item.Id,
            Name = item.Name,
            Image = item.Image,
            SerialNumber = item.SerialNumber,
            Description = item.Description,
            Category = item.Category,
            Quantity = item.Quantity,
            StorageId = item.StorageId,
            StorageName = item.Storage!.Name,
        };
    }

    public async Task<DALDTO.Item?> FindWithStorageAsync(Guid id)
    {
        var res = await RepoDbSet
            .Include(i => i.Storage)
            .FirstOrDefaultAsync(i => i.Id.Equals(id));

        return res != null ? MapDomainToDalDto(res) : null;
    }

    public void Update(DALDTO.Item item)
    {
        RepoDbSet.Update(MapDalDtoToDomain(item));
    }

    private static Domain.Item MapDalDtoToDomain(DALDTO.Item item)
    {
        return new Domain.Item
        {
            Id = item.Id,
            Name = item.Name,
            Image = item.Image,
            SerialNumber = item.SerialNumber,
            Description = item.Description,
            Category = item.Category,
            Quantity = item.Quantity,
            StorageId = item.StorageId
        };
    }

    public void Add(DALDTO.Item item)
    {
        RepoDbSet.Add(MapDalDtoToDomain(item));
    }

    public List<UserItemCount> AllUsersWithItemCount(List<AppUser> users)
    {
        return users
            .Select(user => new
            {
                User = user,
                ItemCount = RepoDbSet.Include(i => i.Storage)
                    .Count(i => i.Storage!.AppUserId.Equals(user.Id))
            })
            .Select(x => new UserItemCount
            {
                Email = x.User.Email!,
                ItemCount = x.ItemCount
            })
            .ToList();
    }
}