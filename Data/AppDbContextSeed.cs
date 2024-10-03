using Domain;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Data;

public static class AppDbContextSeed
{
    public static void SeedAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
        AppDbContext context)
    {
        _ = roleManager.CreateAsync(new AppRole
        {
            Name = "Admin"
        }).Result;

        var adminUser = new AppUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com"
        };

        _ = userManager.CreateAsync(adminUser, "Admin@123").Result;

        _ = userManager.AddToRoleAsync(adminUser, "Admin").Result;

        if (context.Storages.Any())
        {
            return;
        }

        context.Add(new Storage
        {
            Name = "Default Storage", 
            AppUserId = 1
        });

        context.Add(new Storage
        {
            Name = "Another Storage", 
            AppUserId = 1, 
            ParentStorageId = 1
        });

        context.SaveChanges();
    }
}