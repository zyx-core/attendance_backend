using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentAttendance.Data;
using StudentAttendance.Modules.Analytics.Repositories;
using StudentAttendance.Modules.Analytics.Services;
using StudentAttendance.Modules.Dashboard.Services;
using StudentAttendance.Modules.LeaveManagement.Repositories;
using StudentAttendance.Modules.LeaveManagement.Services;
using StudentAttendance.Modules.Notifications.Repositories;
using StudentAttendance.Modules.Notifications.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Database Context =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ===== JWT Authentication =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "DefaultSecretKeyForDevelopment12345!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher", "Admin"));
    options.AddPolicy("ParentOnly", policy => policy.RequireRole("Parent"));
    options.AddPolicy("AllRoles", policy => policy.RequireRole("Teacher", "Parent", "Admin"));
});

// ===== CORS =====
var corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
                  ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", corsBuilder =>
    {
        corsBuilder.WithOrigins(corsOrigins)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
    });
});

// ===== Controllers =====
builder.Services.AddControllers();

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student Attendance Management API",
        Version = "v1",
        Description = "API for managing student attendance, leave requests, notifications, and analytics."
    });

    // JWT auth in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ===== Member 4: Notifications, Leave, Analytics, Dashboard =====
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// ===== Member 1: Authentication (to be added) =====
// builder.Services.AddScoped<IAuthService, AuthService>();

// ===== Member 2: Student Management (to be added) =====
// builder.Services.AddScoped<IStudentService, StudentService>();

// ===== Member 3: Attendance Management (to be added) =====
// builder.Services.AddScoped<IAttendanceService, AttendanceService>();

var app = builder.Build();

// ===== Middleware Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Attendance API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();