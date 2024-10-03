using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public class AppUser : IdentityUser<int>
{
    public ICollection<Storage>? Storages { get; set; }
}