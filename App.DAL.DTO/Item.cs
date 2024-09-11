using App.Constants;
using Base.Contracts.Domain;
using Microsoft.AspNetCore.Http;

namespace App.DAL.DTO;

public class Item : IDomainEntityId
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string ImageName { get; set; }
    public string? SerialNumber { get; set; }
    public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public Guid StorageId { get; set; }
    public required string StorageName { get; set; }
}