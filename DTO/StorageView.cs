using System.ComponentModel.DataAnnotations;

namespace DTO;

public class StorageView
{
    public int Id { get; set; }
    [MaxLength(128)] public required string Name { get; set; }
    public int? ParentStorageId { get; set; }
    public string? ParentStorageName { get; set; }
}