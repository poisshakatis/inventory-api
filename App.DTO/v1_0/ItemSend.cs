using System.ComponentModel.DataAnnotations;
using App.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.DTO.v1_0;

public class ItemSend
{
    [BindNever] public Guid Id { get; set; }
    [MaxLength(128)] public required string Name { get; set; }
    public required string ImageName { get; set; }
    [MaxLength(128)] public string? SerialNumber { get; set; }
    [MaxLength(2048)] public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public Guid StorageId { get; set; }
    public required string StorageName { get; set; }
}