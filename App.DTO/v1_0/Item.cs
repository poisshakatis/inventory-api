using System.ComponentModel.DataAnnotations;
using App.Enums;

namespace App.DTO.v1_0;

public class Item
{
    public Guid Id { get; set; }
    [MaxLength(128)] public required string Name { get; set; }
    public required byte[] Image { get; set; }
    [MaxLength(128)] public string? SerialNumber { get; set; }
    [MaxLength(2048)] public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public Guid StorageId { get; set; }
    public required string StorageName { get; set; }
}