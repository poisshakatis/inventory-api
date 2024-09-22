using App.DTO.v1_0;
using Helpers;
using Item = App.Domain.Item;
using DALDTO = App.DAL.DTO;

namespace App.Services;

public class ItemService
{
    public static Item MapItemReceiveToDomain(ItemReceive item)
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

    public static async Task UploadImage(ItemReceive item)
    {
        var imagePath = FileHelpers.GetImagePath(item.Image.FileName);
        await using var stream = new FileStream(imagePath, FileMode.Create);
        await item.Image.CopyToAsync(stream);
    }

    public static DALDTO.Item MapDomainToDalDto(Item item)
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