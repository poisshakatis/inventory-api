using System.ComponentModel.DataAnnotations;
using Domain.Identity;

namespace Domain;

public class Storage
{
    public int Id { get; set; }
    [MaxLength(128)] public required string Name { get; set; }

    public int? ParentStorageId { get; set; }
    public Storage? ParentStorage { get; set; }

    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}