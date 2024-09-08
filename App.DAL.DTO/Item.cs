using App.Enums;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class Item : IDomainEntityId
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required byte[] Image { get; set; }
    public string? SerialNumber { get; set; }
    public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public Guid StorageId { get; set; }
    public required string StorageName { get; set; }
}