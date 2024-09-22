using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity;

public class AppUser : IdentityUser<Guid>
{
    [MinLength(1)] [MaxLength(64)] public string FirstName { get; set; } = default!;

    [MinLength(1)] [MaxLength(64)] public string LastName { get; set; } = default!;

    public ICollection<Storage>? Storages { get; set; }

    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}