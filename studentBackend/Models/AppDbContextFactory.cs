using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudentAttendance.Models
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            Env.TraversePath().Load();

            var connectionString = BuildConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 0)));

            return new AppDbContext(optionsBuilder.Options);
        }

        private static string BuildConnectionString()
        {
            var explicitConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(explicitConnectionString))
            {
                return explicitConnectionString;
            }

            var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3307";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "attendance";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "appuser";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "appsecret";

            return $"Server={dbServer};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";
        }
    }
}
