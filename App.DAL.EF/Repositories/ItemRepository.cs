using App.Constants;
using App.Contracts.DAL.Repositories;
using App.Domain.Identity;
using App.DTO.v1_0;
using DALDTO = App.DAL.DTO;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Item = App.Domain.Item;

namespace App.DAL.EF.Repositories;

public class ItemRepository : BaseEntityRepository<Domain.Item, AppDbContext>,
    IItemRepository
{
    public ItemRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<DALDTO.Item>> AllWithStorageAsync(Guid userId)
    {
        var res = await GetAllItems(userId);
        return res.Select(MapDomainToDalDto);
    }
    
    private static DALDTO.Item MapDomainToDalDto(Domain.Item item)
    {
        return new DALDTO.Item
        {
            Id = item.Id,
            Name = item.Name,
            ImagePath = item.ImagePath,
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

    public async Task Update(ItemReceive item)
    {
        var oldImagePath = await RepoDbSet
            .Where(i => i.Id.Equals(item.Id))
            .Select(i => i.ImagePath)
            .SingleOrDefaultAsync();

        if (oldImagePath != null)
        {
            File.Delete(oldImagePath);
        }
        
        var newImagePath = GetImagePath(item);

        await UploadImage(item, newImagePath);

        RepoDbSet.Update(MapItemReceiveToDomain(item, newImagePath));
    }

    private static Item MapItemReceiveToDomain(ItemReceive item, string imagePath)
    {
        return new Domain.Item
        {
            Id = item.Id,
            Name = item.Name,
            ImagePath = imagePath,
            SerialNumber = item.SerialNumber,
            Description = item.Description,
            Category = item.Category,
            Quantity = item.Quantity,
            StorageId = item.StorageId
        };
    }

    public async Task Add(ItemReceive item)
    {
        var imagePath = GetImagePath(item);

        await UploadImage(item, imagePath);

        RepoDbSet.Add(MapItemReceiveToDomain(item, imagePath));
    }

    private static async Task UploadImage(ItemReceive item, string imagePath)
    {
        await using var stream = new FileStream(imagePath, FileMode.Create);
        await item.Image.CopyToAsync(stream);
    }

    private static string GetImagePath(ItemReceive item)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var imageName = Path.GetFileName(item.Image.FileName);
        var imagePath = Path.Combine(uploadsFolder, imageName);
        return imagePath;
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
            if (sum > 0)
            {
                res.Add(category, sum);
            }
        }

        return res;
    }

    private async Task<List<Item>> GetAllItems(Guid userId)
    {
        return await RepoDbSet
            .Include(i => i.Storage)
            .Where(i => i.Storage!.AppUserId.Equals(userId))
            .ToListAsync();
    }
}