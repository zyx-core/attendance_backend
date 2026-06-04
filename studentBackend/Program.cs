using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentAttendance.Models;
using StudentAttendance.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. ALL SERVICE REGISTRATIONS (Must be BEFORE builder.Build())
// =========================================================================

// ===== Database Context =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ===== Controller & Routing Services =====
builder.Services.AddControllers();

// ===== Business Logic & Custom Services =====
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IEmailService, EmailService>(); // FIXED: Moved here before Build()

// ===== Basic Authorization Setup =====
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnly", policy => policy.RequireAssertion(_ => true)); 
    options.AddPolicy("AllRoles", policy => policy.RequireAssertion(_ => true));   
});


// =========================================================================
// 2. BUILD THE APPLICATION (Locks the service collection)
// =========================================================================
var app = builder.Build();


// =========================================================================
// 3. HTTP REQUEST PIPELINE CONFIGURATION (Middleware)
// =========================================================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Map routes for endpoints
app.MapControllers();

// Run the web host
app.Run();