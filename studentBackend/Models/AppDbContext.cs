using Microsoft.EntityFrameworkCore;

namespace StudentAttendance.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Register your LeaveRequest model so EF Core creates the table
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Student> Students { get; set; }

        // Note: As you build out your application, you will add your other models here, like:
        // public DbSet<Attendance> Attendances { get; set; }
    }
}
