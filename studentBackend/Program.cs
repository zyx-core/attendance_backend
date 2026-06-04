using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StudentAttendance.Models;
using StudentAttendance.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var frontendOriginsFromEnv = Environment.GetEnvironmentVariable("FRONTEND_ORIGINS");
var frontendOrigins = !string.IsNullOrWhiteSpace(frontendOriginsFromEnv)
    ? frontendOriginsFromEnv.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
    : builder.Configuration.GetSection("Frontend:Origins").Get<string[]>()
        ?? ["http://localhost:4200", "http://127.0.0.1:4200"];

// ===== 1. Database Context Registration =====
var configuredConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionStringFromEnv = Environment.GetEnvironmentVariable("CONNECTION_STRING");
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var connectionString =
    !string.IsNullOrWhiteSpace(connectionStringFromEnv)
        ? connectionStringFromEnv
        : !string.IsNullOrWhiteSpace(dbServer) &&
          !string.IsNullOrWhiteSpace(dbPort) &&
          !string.IsNullOrWhiteSpace(dbName) &&
          !string.IsNullOrWhiteSpace(dbUser)
            ? $"Server={dbServer};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};"
            : configuredConnectionString;

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured. Set CONNECTION_STRING or DB_SERVER/DB_PORT/DB_NAME/DB_USER/DB_PASSWORD.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

// ===== 2. Controller & Routing Services =====
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendApp", policy =>
    {
        policy.WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ===== 3. Swagger Services =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer your_token_here"
    });

});

// ===== 4. Service Registrations =====
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IStudentService, StudentService>();

// ===== 5. JWT Authentication & Authorization =====
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? "";
if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
{
    if (builder.Environment.IsDevelopment())
    {
        secretKey = "this_is_a_development_secret_key_for_local_sams_only";
    }
    else
    {
        throw new InvalidOperationException("JWT secret must be configured and at least 32 characters in non-development environments.");
    }
}

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"];
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
        ValidIssuer = jwtIssuer,
        ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnly",
        policy => policy.RequireRole("Teacher"));

    options.AddPolicy("AllRoles",
        policy => policy.RequireRole("Teacher", "Parent"));
});

var app = builder.Build();

// ===== 6. HTTP Pipeline Configuration =====
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            if (exceptionHandler?.Error != null)
            {
                logger.LogError(exceptionHandler.Error, "Unhandled API exception");
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://httpstatuses.com/500",
                title = "An unexpected error occurred.",
                status = StatusCodes.Status500InternalServerError
            });
        });
    });
}

app.UseCors("FrontendApp");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Attendance API is running successfully!");

// ===== 7. Execution Loop =====
app.Run();
