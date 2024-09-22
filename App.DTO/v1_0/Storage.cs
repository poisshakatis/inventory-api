using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1_0;

public class Storage
{
    public Guid Id { get; set; }
    [MaxLength(128)] public required string Name { get; set; }
    public Guid? ParentStorageId { get; set; }
    public string? ParentStorageName { get; set; }
}