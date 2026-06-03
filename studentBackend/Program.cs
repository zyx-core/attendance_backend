using Microsoft.EntityFrameworkCore;
using StudentAttendance.Models; // Ensure this matches where your AppDbContext lives!
using StudentAttendance.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Database Context Registration =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ===== Service Registrations =====
builder.Services.AddScoped<ILeaveService, LeaveService>();

// ... the rest of your Program.cs setup below