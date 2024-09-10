using App.Constants;
using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AppDbContext(DbContextOptions options)
    : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, AppUserRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
    public DbSet<Storage> Storages { get; init; }
    public DbSet<Item> Items { get; init; }
    public DbSet<AppRefreshToken> RefreshTokens { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Item>()
            .Property(i => i.Category)
            .HasConversion(
                c => c.ToString(),
                c => (Category)Enum.Parse(typeof(Category), c));

        builder.Entity<Item>()
            .Property(i => i.Quantity)
            .HasDefaultValue(1);
    }
}