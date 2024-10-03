using System.ComponentModel.DataAnnotations;
using Constants;
using Microsoft.AspNetCore.Http;

namespace DTO;

public class SaveItemRequest
{
    [MaxLength(128)] public required string Name { get; set; }
    public required IFormFile Image { get; set; }
    [MaxLength(128)] public string? SerialNumber { get; set; }
    [MaxLength(2048)] public required string Description { get; set; }
    public Category Category { get; set; }
    public int Quantity { get; set; }
    public int StorageId { get; set; }
}