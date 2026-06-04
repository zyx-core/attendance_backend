using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StudentAttendance.Models;
using StudentAttendance.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var frontendOrigins = builder.Configuration
    .GetSection("Frontend:Origins")
    .Get<string[]>() ?? ["http://localhost:4200", "http://127.0.0.1:4200"];

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
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return uri.Port == 4200 &&
                    (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                     uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase));
            })
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

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===== 4. Service Registrations =====
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IStudentService, StudentService>();

// ===== 5. JWT Authentication & Authorization =====
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? "this_is_a_fallback_secret_key_that_must_be_long_enough_for_hmac_sha256";
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
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnly",
        policy => policy.RequireAssertion(_ => true));

    options.AddPolicy("AllRoles",
        policy => policy.RequireAssertion(_ => true));
});

var app = builder.Build();

// ===== 6. HTTP Pipeline Configuration =====
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
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
