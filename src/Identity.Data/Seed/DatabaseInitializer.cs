using Identity.Data.Context;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
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
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();
        var context = services.GetRequiredService<AppDbContext>();

        const int maxRetries = 3;
        var delay = TimeSpan.FromSeconds(3);
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation($"[{attempt}/{maxRetries}] Checking database connection...");

                await EnsureDatabaseExistsAsync(connectionString, logger);

                logger.LogInformation("Ensuring database schema is up to date...");
                await context.Database.MigrateAsync();

                var userManager = services.GetRequiredService<UserManager<User>>();
                await SeedAdminUserAsync(userManager, logger, configuration);

                logger.LogInformation("Database initialization complete.");
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Database initialization failed on attempt {attempt}. Retrying in {delay.TotalSeconds}s...");
                if (attempt == maxRetries)
                {
                    logger.LogError(ex, "Could not initialize the database after multiple retries.");
                    throw;
                }

                await Task.Delay(delay);
            }
        }
    }

    private static async Task EnsureDatabaseExistsAsync(string connectionString, ILogger logger)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = $"SELECT database_id FROM sys.databases WHERE Name = '{databaseName}'";
        var result = await checkCommand.ExecuteScalarAsync();

        if (result == null)
        {
            logger.LogInformation($"Database '{databaseName}' does not exist. Creating...");
            var createCommand = connection.CreateCommand();
            createCommand.CommandText = $"CREATE DATABASE [{databaseName}]";
            await createCommand.ExecuteNonQueryAsync();
            logger.LogInformation($"Database '{databaseName}' created successfully!");
        }
        else
        {
            logger.LogInformation($"Database '{databaseName}' already exists.");
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

            var newUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, adminPassword);
            if (result.Succeeded)
                logger.LogInformation("Admin user created successfully!");
            else
                foreach (var error in result.Errors)
                    logger.LogError($"{error.Description}");
        }
        else
        {
            logger.LogInformation("Admin user already exists.");
        }
    }
}