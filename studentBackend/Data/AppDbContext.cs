using Microsoft.EntityFrameworkCore;
using StudentAttendance.Data.Entities;

namespace StudentAttendance.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<ParentStudent> ParentStudents { get; set; } = null!;
    public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
    public DbSet<ParentInvitation> ParentInvitations { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<AttendanceSummary> AttendanceSummaries { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== User =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Role)
                  .HasConversion<string>()
                  .HasMaxLength(20);
        });

        // ===== Student =====
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.RollNumber).IsUnique();
            entity.HasOne(s => s.Creator)
                  .WithMany(u => u.CreatedStudents)
                  .HasForeignKey(s => s.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Attendance =====
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasIndex(a => new { a.StudentId, a.AttendanceDate }).IsUnique();
            entity.Property(a => a.Status)
                  .HasConversion<string>()
                  .HasMaxLength(10);
            entity.HasOne(a => a.Student)
                  .WithMany(s => s.Attendances)
                  .HasForeignKey(a => a.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(a => a.MarkedByUser)
                  .WithMany(u => u.MarkedAttendances)
                  .HasForeignKey(a => a.MarkedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== ParentStudent =====
        modelBuilder.Entity<ParentStudent>(entity =>
        {
            entity.HasIndex(ps => new { ps.ParentId, ps.StudentId }).IsUnique();
            entity.HasOne(ps => ps.Parent)
                  .WithMany(u => u.ParentStudents)
                  .HasForeignKey(ps => ps.ParentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ps => ps.Student)
                  .WithMany(s => s.ParentStudents)
                  .HasForeignKey(ps => ps.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== LeaveRequest =====
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.Property(lr => lr.Status)
                  .HasConversion<string>()
                  .HasMaxLength(20);
            entity.HasOne(lr => lr.Student)
                  .WithMany(s => s.LeaveRequests)
                  .HasForeignKey(lr => lr.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(lr => lr.Parent)
                  .WithMany(u => u.LeaveRequestsAsParent)
                  .HasForeignKey(lr => lr.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(lr => lr.Reviewer)
                  .WithMany(u => u.ReviewedLeaveRequests)
                  .HasForeignKey(lr => lr.ReviewedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== ParentInvitation =====
        modelBuilder.Entity<ParentInvitation>(entity =>
        {
            entity.HasIndex(pi => pi.InviteToken).IsUnique();
            entity.HasOne(pi => pi.Student)
                  .WithMany(s => s.Invitations)
                  .HasForeignKey(pi => pi.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(pi => pi.Creator)
                  .WithMany(u => u.CreatedInvitations)
                  .HasForeignKey(pi => pi.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Notification =====
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(n => n.Type)
                  .HasConversion<string>()
                  .HasMaxLength(30);
            entity.HasOne(n => n.Student)
                  .WithMany(s => s.Notifications)
                  .HasForeignKey(n => n.StudentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ===== AttendanceSummary =====
        modelBuilder.Entity<AttendanceSummary>(entity =>
        {
            entity.HasIndex(asummary => asummary.StudentId).IsUnique();
            entity.Property(asummary => asummary.RiskLevel)
                  .HasConversion<string>()
                  .HasMaxLength(10);
            entity.HasOne(asummary => asummary.Student)
                  .WithOne(s => s.AttendanceSummary)
                  .HasForeignKey<AttendanceSummary>(asummary => asummary.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== AuditLog =====
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasOne(al => al.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(al => al.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
