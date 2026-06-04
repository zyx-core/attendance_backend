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
        public DbSet<User> Users { get; set; }
        public DbSet<ParentInvitation> ParentInvitations { get; set; }
        public DbSet<ParentStudent> ParentStudent { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    }
}