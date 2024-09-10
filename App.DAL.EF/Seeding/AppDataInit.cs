using App.Constants;
using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Seeding;

public static class AppDataInit
{
    private const string AdminEmail = "admin@eesti.ee";
    

    public static void SeedAppData(UserManager<AppUser> userManager, AppDbContext context)
    {
        var user = userManager.FindByEmailAsync(AdminEmail).Result;
        var parentStorageId = Guid.Parse("ecea052c-c48a-4e6f-9c32-71f3ca0a97b5");
        
        if (user != null && !context.Storages.Any())
        {
            context.Add(new Storage
            {
                Name = "kuur",
                AppUserId = user.Id,
                Id = parentStorageId
            });
            context.Add(new Storage
            {
                Name = "kapp",
                AppUserId = user.Id,
                ParentStorageId = parentStorageId
            });
            context.SaveChanges();
        }
    }

    public static void MigrateDatabase(AppDbContext context)
    {
        context.Database.Migrate();
    }

    public static void DeleteDatabase(AppDbContext context)
    {
        context.Database.EnsureDeleted();
    }
        
    public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        var res = roleManager.CreateAsync(new AppRole
        {
            Name = "Admin"
        }).Result;

        if (!res.Succeeded)
        {
            Console.WriteLine(res.ToString());
        }
        
        var user = userManager.FindByEmailAsync(AdminEmail).Result;

        if (user != null) return;

        user = new AppUser
        {
            Email = "admin@eesti.ee",
            UserName = "admin@eesti.ee",
            FirstName = "Admin",
            LastName = "Eesti"
        };
        
        res = userManager.CreateAsync(user, "Kala.maja1").Result;
        if (!res.Succeeded)
        {
            Console.WriteLine(res.ToString());
        }

        res = userManager.AddToRoleAsync(user, "Admin").Result;
        if (!res.Succeeded)
        {
            Console.WriteLine(res.ToString());
        }
    }

}