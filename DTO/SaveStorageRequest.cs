using System.ComponentModel.DataAnnotations;

namespace DTO;

public class SaveStorageRequest
{
    [MaxLength(128)] public required string Name { get; set; }
    public int? ParentStorageId { get; set; }
}