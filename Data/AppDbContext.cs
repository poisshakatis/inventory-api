using Constants;
using Domain;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext(DbContextOptions options)
    : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, IdentityUserRole<int>,
        IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    public DbSet<Storage> Storages { get; init; }
    public DbSet<Item> Items { get; init; }
    public DbSet<RefreshToken> RefreshTokens { get; init; }

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