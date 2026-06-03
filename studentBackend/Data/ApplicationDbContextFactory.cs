using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace studentBackend.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }

        var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "attendance";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty;

        var connectionString = $"server={dbServer};port={dbPort};database={dbName};user={dbUser};password={dbPassword}";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}