using LearnWellUniversity_CMS.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnWellUniversity_CMS.Infrastructure.DataAccess;

public class AppDbContext(DbContextOptions<AppDbContext> options)
: IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<CourseClass> CourseClasses => Set<CourseClass>();
    public DbSet<StudentCourse> StudentCourses => Set<StudentCourse>();
    public DbSet<StudentClass> StudentClasses => Set<StudentClass>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(u => u.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(u => u.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasOne(u => u.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(u => u.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasOne(u => u.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(u => u.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.UserId).IsUnique();
            entity.HasOne(s => s.User)
                    .WithOne()
                    .HasForeignKey<Student>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(u => u.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(u => u.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CourseClass>(entity =>
        {
            entity.HasKey(cc => new { cc.CourseId, cc.ClassId });
            entity.HasOne(cc => cc.Course).WithMany(c => c.CourseClasses).HasForeignKey(cc => cc.CourseId);
            entity.HasOne(cc => cc.Class).WithMany(c => c.CourseClasses).HasForeignKey(cc => cc.ClassId);
            entity.HasOne(sc => sc.AssignedByUser).WithMany().HasForeignKey(sc => sc.AssignedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasIndex(sc => new { sc.StudentId, sc.CourseId }).IsUnique();
            entity.HasOne(sc => sc.Student).WithMany(s => s.StudentCourses).HasForeignKey(sc => sc.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(sc => sc.Course).WithMany(c => c.StudentCourses).HasForeignKey(sc => sc.CourseId);
            entity.HasOne(sc => sc.AssignedByUser).WithMany().HasForeignKey(sc => sc.AssignedBy).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StudentClass>(entity =>
        {
            entity.HasIndex(sc => new { sc.StudentId, sc.ClassId }).IsUnique();
            entity.HasOne(sc => sc.Student).WithMany(s => s.StudentClasses).HasForeignKey(sc => sc.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(sc => sc.Class).WithMany(c => c.StudentClasses).HasForeignKey(sc => sc.ClassId);
            entity.HasOne(sc => sc.AssignedByUser).WithMany().HasForeignKey(sc => sc.AssignedBy).OnDelete(DeleteBehavior.Restrict);
        });
    }
}