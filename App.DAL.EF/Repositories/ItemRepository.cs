using App.Constants;
using App.Contracts.DAL.Repositories;
using App.Domain.Identity;
using App.DTO.v1_0;
using DALDTO = App.DAL.DTO;
using Base.DAL.EF;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Item = App.Domain.Item;

namespace App.DAL.EF.Repositories;

public class ItemRepository : BaseEntityRepository<Item, AppDbContext>,
    IItemRepository
{
    public ItemRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<DALDTO.Item>> AllWithStorageAsync(Guid userId, string? query, int? limit)
    {
        var res = await GetAllItems(userId, query, limit);
        return res.Select(MapDomainToDalDto);
    }

    public async Task<DALDTO.Item?> FindWithStorageAsync(Guid id)
    {
        var res = await RepoDbSet
            .Include(i => i.Storage)
            .FirstOrDefaultAsync(i => i.Id.Equals(id));

        return res != null ? MapDomainToDalDto(res) : null;
    }

    public async Task Update(ItemReceive item)
    {
        var oldImageName = await RepoDbSet
            .Where(i => i.Id.Equals(item.Id))
            .Select(i => i.ImageName)
            .SingleOrDefaultAsync();

        if (oldImageName != null)
        {
            var oldImagePath = FileHelpers.GetImagePath(oldImageName);

            File.Delete(oldImagePath);
        }

        await UploadImage(item);

        RepoDbSet.Update(MapItemReceiveToDomain(item));
    }

    public async Task Add(ItemReceive item)
    {
        await UploadImage(item);
        RepoDbSet.Add(MapItemReceiveToDomain(item));
    }

    public async Task<List<UserCategoryItemCount>> AllUsersWithCategoryItemCount(List<AppUser> users)
    {
        var res = new List<UserCategoryItemCount>();
        foreach (var user in users)
        {
            var items = await GetAllItems(user.Id);
            res.Add(new UserCategoryItemCount
            {
                Email = user.Email!,
                CategoryItemCount = CountAllCategoryItems(items)
            });
        }

        return res;
    }

    private static Dictionary<Category, int> CountAllCategoryItems(List<Item> items)
    {
        var res = new Dictionary<Category, int>();
        foreach (var category in Enum.GetValues<Category>())
        {
            var sum = items.Where(i => i.Category == category).Sum(i => i.Quantity);
            if (sum > 0) res.Add(category, sum);
        }

        return res;
    }

    private async Task<List<Item>> GetAllItems(Guid userId, string? query = null, int? limit = null)
    {
        var baseQuery = RepoDbSet
            .Include(i => i.Storage)
            .Where(i => i.Storage!.AppUserId.Equals(userId));

        if (!string.IsNullOrEmpty(query))
            baseQuery = baseQuery.Where(i =>
                i.Name.Contains(query) ||
                (i.SerialNumber != null && i.SerialNumber.Contains(query)) ||
                i.Description.Contains(query) ||
                (i.Storage != null && i.Storage.Name.Contains(query)));

        if (limit.HasValue) baseQuery = baseQuery.Take(limit.Value);

        return await baseQuery.ToListAsync();
    }

    private static Item MapItemReceiveToDomain(ItemReceive item)
    {
        return new Item
        {
            Id = item.Id,
            Name = item.Name,
            ImageName = item.Image.FileName,
            SerialNumber = item.SerialNumber,
            Description = item.Description,
            Category = item.Category,
            Quantity = item.Quantity,
            StorageId = item.StorageId
        };
    }

    private static async Task UploadImage(ItemReceive item)
    {
        var imagePath = FileHelpers.GetImagePath(item.Image.FileName);
        await using var stream = new FileStream(imagePath, FileMode.Create);
        await item.Image.CopyToAsync(stream);
    }

    private static DALDTO.Item MapDomainToDalDto(Item item)
    {
        return new DALDTO.Item
        {
            Id = item.Id,
            Name = item.Name,
            ImageName = item.ImageName,
            SerialNumber = item.SerialNumber,
            Description = item.Description,
            Category = item.Category,
            Quantity = item.Quantity,
            StorageId = item.StorageId,
            StorageName = item.Storage!.Name
        };
    }
}