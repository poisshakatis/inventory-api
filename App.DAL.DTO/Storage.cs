using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class Storage : IDomainEntityId
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid? ParentStorageId { get; set; }
    public string? ParentStorageName { get; set; }
}