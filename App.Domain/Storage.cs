using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Contracts.Domain;
using Base.Domain;

namespace App.Domain;

public class Storage : BaseEntityId, IDomainAppUser<AppUser>
{
    [MaxLength(128)] public required string Name { get; set; }

    public Guid? ParentStorageId { get; set; }
    public Storage? ParentStorage { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}