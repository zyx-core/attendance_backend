using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentAttendance.Models;
using StudentAttendance.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. Database Context Registration =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString, 
        ServerVersion.AutoDetect(connectionString)
    ));

// ===== 2. Controller & Routing Services =====
builder.Services.AddControllers();

// ===== 3. Swagger Services (For Local Endpoint Testing) =====
builder.Services.AddEndpointsApiExplorer();


// ===== 4. Core System Service Registrations =====
builder.Services.AddScoped<ILeaveService, LeaveService>();

builder.Services.AddScoped<IAnalyticsService, AnalyticsService>(); // Retained analytics setup

// ===== 5. Authorization Policies =====
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnly", policy => policy.RequireAssertion(_ => true)); // Local testing placeholder bypass
    options.AddPolicy("AllRoles", policy => policy.RequireAssertion(_ => true));   // Local testing placeholder bypass
});

// ===== 6. CORS Policy Registration =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// ===== 7. HTTP Pipeline Configuration =====


app.UseHttpsRedirection();

// CRITICAL CORRECT MIDDLEWARE ORDERING:
app.UseRouting();

// UseCors MUST always execute after routing but BEFORE mapping controllers and authorization pipelines
app.UseCors("AllowAngularFrontend");

app.UseAuthorization();

// Maps API endpoint routing matrices (e.g., api/Leave and api/Analytics)
app.MapControllers();

// ===== 8. Execution Loop =====
app.Run();