using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentAttendance.Models;
using StudentAttendance.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. Database Context Registration =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ===== 2. Controller & Routing Services =====
builder.Services.AddControllers();

// ===== 3. Service Registrations =====
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

// ===== 4. Basic Auth & Authorization Layout =====
// Keeping placeholders matching the policies you have configured on your Controller
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeacherOnly", policy => policy.RequireAssertion(_ => true)); // Temporary bypass for local testing
    options.AddPolicy("AllRoles", policy => policy.RequireAssertion(_ => true));   // Temporary bypass for local testing
});

var app = builder.Build();

// ===== 5. HTTP Pipeline Configuration =====
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Maps controller routes so 'api/Leave' endpoints can be reached
app.MapControllers();

// ===== 6. Execution Loop =====
app.Run();