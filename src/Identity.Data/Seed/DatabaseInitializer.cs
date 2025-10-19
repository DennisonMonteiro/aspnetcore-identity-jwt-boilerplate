using Identity.Data.Context;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Data.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();

            logger.LogInformation("Checking for pending migrations...");
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully!");
            }
            else
            {
                logger.LogInformation("Database is up to date.");
            }

            await SeedAdminUserAsync(userManager, logger, configuration);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<User> userManager, ILogger logger, IConfiguration configuration)
    {
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@identity.com";
        var adminPassword = configuration["AdminUser:Password"] ?? "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            logger.LogInformation("Creating admin user...");

            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                logger.LogInformation("Admin user created successfully!");
                logger.LogInformation($"Email: {adminEmail}");
                logger.LogInformation($"Password: {adminPassword}");
            }
            else
            {
                logger.LogError("Failed to create admin user:");
                foreach (var error in result.Errors)
                {
                    logger.LogError($"- {error.Description}");
                }
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists.");
        }
    }
}