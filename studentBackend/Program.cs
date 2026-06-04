using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentAttendance.Models;
using StudentAttendance.Services;

var builder = WebApplication.CreateBuilder(args);
var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:4200";

// ===== 1. Database Context Registration =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ===== 3. Swagger Services =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== 4. Service Registrations =====
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IStudentService, StudentService>();

// ===== 5. Basic Auth & Authorization Layout =====
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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("FrontendApp");
app.UseAuthorization();
app.MapControllers();

// ===== 7. Execution Loop =====
app.Run();
