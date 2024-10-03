using Data;
using Domain;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Services;

public class ItemService(AppDbContext context)
{
    public async Task<List<ItemView>> GetItems(int userId, string? query = null, int? limit = null)
    {
        var baseQuery = context.Items
            .Where(i => i.Storage.AppUserId.Equals(userId));

        if (!string.IsNullOrEmpty(query))
        {
            baseQuery = baseQuery.Where(i =>
                i.Name.Contains(query) ||
                (i.SerialNumber != null && i.SerialNumber.Contains(query)) ||
                i.Description.Contains(query) ||
                i.Storage.Name.Contains(query));
        }

        if (limit.HasValue)
        {
            baseQuery = baseQuery.Take(limit.Value);
        }

        return await baseQuery.Select(i => new ItemView
        {
            Id = i.Id,
            Name = i.Name,
            ImageName = i.ImageName,
            SerialNumber = i.SerialNumber,
            Description = i.Description,
            Category = i.Category,
            Quantity = i.Quantity,
            StorageId = i.StorageId,
            StorageName = i.Storage.Name
        }).ToListAsync();
    }

    public async Task<Item?> Find(int id)
    {
        return await context.Items
            .Include(i => i.Storage)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task Update(Item item, SaveItemRequest request)
    {
        var oldImagePath = ImageUtils.GetImagePath(item.ImageName);

        File.Delete(oldImagePath);

        // Upload image
        var imagePath = ImageUtils.GetImagePath(request.Image.FileName);
        await using var stream = new FileStream(imagePath, FileMode.Create);
        await request.Image.CopyToAsync(stream);

        item.Name = request.Name;
        item.ImageName = request.Image.FileName;
        item.SerialNumber = request.SerialNumber;
        item.Description = request.Description;
        item.Category = request.Category;
        item.Quantity = request.Quantity;
        item.StorageId = request.StorageId;

        context.Update(item);
        await context.SaveChangesAsync();
    }
}