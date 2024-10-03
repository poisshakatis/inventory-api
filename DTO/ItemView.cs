using System.ComponentModel.DataAnnotations;
using Constants;

namespace DTO;

public class ItemView
{
    public int Id { get; set; }
    [MaxLength(128)] public required string Name { get; set; }
    public required string ImageName { get; set; }
    [MaxLength(128)] public string? SerialNumber { get; set; }
    [MaxLength(2048)] public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public int StorageId { get; set; }
    public required string StorageName { get; set; }
}