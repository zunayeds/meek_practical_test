using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnWellUniversity_CMS.Infrastructure.DataAccess
{
    internal static class DataMigrationExtensions
    {
        private static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid AnotherAdminUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid StaffRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid StudentRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        private static readonly Guid ProgrammingCourseId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        private static readonly Guid BusinessCourseId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        private static readonly Guid MarketingCourseId = Guid.Parse("77777777-7777-7777-7777-777777777777");

        private static readonly Guid Math101ClassId = Guid.Parse("88888888-8888-8888-8888-888888888888");
        private static readonly Guid Bus101ClassId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        private static readonly Guid Mkt101ClassId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid Eth101ClassId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        private static readonly DateTimeOffset CreatedAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private const string AdminEmail = "admin@learnwell.edu";
        private const string AnotherAdminEmail = "admin2@learnwell.edu";
        private const string AdminEmailNormalized = "ADMIN@LEARNWELL.EDU";
        private const string AnotherAdminEmailNormalized = "ADMIN2@LEARNWELL.EDU";
        private const string AdminConcurrencyStamp = "aaaaaaaa-aaaa-aaaa-aaaa-000000000001";
        private const string AnotherAdminConcurrencyStamp = "aaaaaaaa-aaaa-aaaa-aaaa-000000000002";
        private const string AdminPasswordHash = "AQAAAAIAAYagAAAAEDWQ6ElmXAaZfrmYWWCViK0+DQ+ObIRYZw6DkP1mbNibQfwaPocZhTzeb7W+RLQnBA==";

        public static ModelBuilder SeedInitialData(this ModelBuilder modelBuilder)
        {
            SeedUserData(modelBuilder);
            SeedCourseClassData(modelBuilder);

            return modelBuilder;
        }

        private static void SeedUserData(ModelBuilder modelBuilder)
        {
            var staffRole = new IdentityRole<Guid>
            {
                Id = StaffRoleId,
                Name = Roles.Staff,
                NormalizedName = Roles.Staff.ToUpperInvariant()
            };

            var studentRole = new IdentityRole<Guid>
            {
                Id = StudentRoleId,
                Name = Roles.Student,
                NormalizedName = Roles.Student.ToUpperInvariant()
            };

            var admin = new ApplicationUser
            {
                Id = AdminUserId,
                FirstName = "Super",
                LastName = "Admin",
                UserName = AdminEmail,
                NormalizedUserName = AdminEmailNormalized,
                Email = AdminEmail,
                NormalizedEmail = AdminEmailNormalized,
                EmailConfirmed = true,
                CreatedAt = CreatedAt,
                CreatedBy = AdminUserId,
                PasswordHash = AdminPasswordHash,
                ConcurrencyStamp = AdminConcurrencyStamp
            };

            var anotherAdmin = new ApplicationUser
            {
                Id = AnotherAdminUserId,
                FirstName = "Another",
                LastName = "Admin",
                UserName = AnotherAdminEmail,
                NormalizedUserName = AnotherAdminEmailNormalized,
                Email = AnotherAdminEmail,
                NormalizedEmail = AnotherAdminEmailNormalized,
                EmailConfirmed = true,
                CreatedAt = CreatedAt,
                CreatedBy = AdminUserId,
                PasswordHash = AdminPasswordHash,
                ConcurrencyStamp = AnotherAdminConcurrencyStamp
            };

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(staffRole, studentRole);
            modelBuilder.Entity<ApplicationUser>().HasData(admin, anotherAdmin);
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid> { UserId = AdminUserId, RoleId = StaffRoleId },
                new IdentityUserRole<Guid> { UserId = AnotherAdminUserId, RoleId = StaffRoleId }
            );
        }

        private static void SeedCourseClassData(ModelBuilder modelBuilder)
        {
            var courses = new List<Course>
            {
                new() { CourseId = ProgrammingCourseId, Name = "Programming", Description = "Programming Course", CreatedAt = CreatedAt, CreatedBy = AdminUserId },
                new() { CourseId = BusinessCourseId, Name = "Business", Description = "Business Course", CreatedAt = CreatedAt, CreatedBy = AdminUserId },
                new() { CourseId = MarketingCourseId, Name = "Marketing", Description = "Marketing Course", CreatedAt = CreatedAt, CreatedBy = AdminUserId }
            };
            modelBuilder.Entity<Course>().HasData(courses);

            var classes = new List<Class>
            {
                new() { ClassId = Math101ClassId, Name = "Math 101", Description = "Basic Math", CreatedAt = CreatedAt, CreatedBy = AdminUserId },
                new() { ClassId = Bus101ClassId, Name = "Bus 101", Description = "Business Basics", CreatedAt = CreatedAt, CreatedBy = AdminUserId },
                new() { ClassId = Mkt101ClassId, Name = "Mkt 101", Description = "Marketing Basic", CreatedAt = CreatedAt, CreatedBy = AdminUserId },
                new() { ClassId = Eth101ClassId, Name = "Eth 101", Description = "General Ethics", CreatedAt = CreatedAt, CreatedBy = AdminUserId }
            };
            modelBuilder.Entity<Class>().HasData(classes);

            var courseClasses = new List<CourseClass>
            {
                new() { CourseId = ProgrammingCourseId, ClassId = Math101ClassId, AssignedAt = CreatedAt, AssignedBy = AdminUserId },
                new() { CourseId = BusinessCourseId, ClassId = Math101ClassId, AssignedAt = CreatedAt, AssignedBy = AdminUserId },
                new() { CourseId = BusinessCourseId, ClassId = Bus101ClassId, AssignedAt = CreatedAt, AssignedBy = AdminUserId },
                new() { CourseId = MarketingCourseId, ClassId = Mkt101ClassId, AssignedAt = CreatedAt, AssignedBy = AdminUserId }
            };
            modelBuilder.Entity<CourseClass>().HasData(courseClasses);
        }
    }
}
