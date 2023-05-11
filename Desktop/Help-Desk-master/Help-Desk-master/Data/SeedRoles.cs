using HelpDesk.Enums;
using HelpDesk.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Data;

public abstract class SeedRoles
{
    public static async void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Roles.Any())
        {
            return;
        }

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var roleList = new List<IdentityRole>
            {
                new() { Name = nameof(Roles.Technician), NormalizedName = nameof(Roles.Technician).ToUpper() },
                new() { Name = nameof(Roles.User), NormalizedName = nameof(Roles.User) },
            };

            context.Roles.AddRange(roleList);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error seeding data", ex);
        }
    }

    public static async Task CreateDefaultUser(IServiceProvider serviceProvider)
    {
        await using var context =
            new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (userManager.FindByEmailAsync("admin@example.com").Result != null) return;
        var user = new User
        {
            UserName = "admin@example.com",
            Email = "admin@example.com"
        };
        const string password = "achref1234";
        var result = userManager.CreateAsync(user, password).Result;
        if (!result.Succeeded) return;
        var adminRole = roleManager.FindByNameAsync(nameof(Roles.Technician)).Result;
        await userManager.AddToRoleAsync(user, adminRole.Name);
    }
}