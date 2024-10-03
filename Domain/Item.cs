using System.ComponentModel.DataAnnotations;
using Constants;

namespace Domain;

public class Item
{
    public int Id { get; set; }
    [MaxLength(128)] public required string Name { get; set; }
    [MaxLength(256)] public required string ImageName { get; set; }
    [MaxLength(128)] public string? SerialNumber { get; set; }
    [MaxLength(2048)] public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public int StorageId { get; set; }
    public Storage? Storage { get; set; }
}