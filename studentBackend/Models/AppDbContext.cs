using Microsoft.EntityFrameworkCore;

namespace StudentAttendance.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ParentInvitation> ParentInvitations { get; set; }
        public DbSet<ParentStudent> ParentStudent { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    }
}
