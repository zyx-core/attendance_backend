using Microsoft.EntityFrameworkCore;
using studentBackend.Models;

namespace studentBackend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");

            entity.HasKey(student => student.Id);

            entity.Property(student => student.RollNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(student => student.RollNumber)
                .IsUnique();

            entity.Property(student => student.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(student => student.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(student => student.ClassName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(student => student.Division)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(student => student.StudentEmail)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(student => student.StudentEmail)
                .IsUnique();

            entity.Property(student => student.PhotoUrl)
                .HasMaxLength(500);

            entity.Property(student => student.IsActive)
                .HasDefaultValue(true);

            entity.Property(student => student.CreatedAt)
                .IsRequired();

            entity.Property(student => student.UpdatedAt)
                .IsRequired();
        });
    }
}
