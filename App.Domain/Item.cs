using System.ComponentModel.DataAnnotations;
using App.Enums;
using Base.Domain;

namespace App.Domain;

public class Item : BaseEntityId
{
    [MaxLength(128)] public required string Name { get; set; }
    public required byte[] Image { get; set; }
    [MaxLength(128)] public string? SerialNumber { get; set; }
    [MaxLength(2048)] public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public Guid StorageId { get; set; }
    public Storage? Storage { get; set; }
}