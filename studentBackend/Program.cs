using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using studentBackend.Data;
using studentBackend.Services;

var builder = WebApplication.CreateBuilder(args);

var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuredFrontendOrigins = (Environment.GetEnvironmentVariable("FRONTEND_ORIGINS") ?? string.Empty)
    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var frontendOrigins = configuredFrontendOrigins
    .Append("http://localhost:4200")
    .Append("http://127.0.0.1:4200")
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (string.IsNullOrWhiteSpace(dbServer) ||
    string.IsNullOrWhiteSpace(dbPort) ||
    string.IsNullOrWhiteSpace(dbName) ||
    string.IsNullOrWhiteSpace(dbUser) ||
    dbPassword is null)
{
    throw new InvalidOperationException("Database connection environment variables are not configured.");
}

var connectionString =
    $"server={dbServer};port={dbPort};database={dbName};user={dbUser};password={dbPassword}";

builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var configuredConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(configuredConnectionString))
    {
        throw new InvalidOperationException("Database connection environment variables are not configured.");
    }

    options.UseMySql(configuredConnectionString, new MySqlServerVersion(new Version(8, 0, 36)));
});

builder.Services.AddScoped<StudentService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("FrontendPolicy");
app.MapControllers().RequireCors("FrontendPolicy");

app.Run();
