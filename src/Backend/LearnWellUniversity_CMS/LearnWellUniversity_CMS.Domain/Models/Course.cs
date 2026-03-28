using LearnWellUniversity_CMS.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Domain.Models;

public class Course : AuditTrailBase
{
    [Key]
    public Guid CourseId { get; set; }

    [StringLength(30)]
    public required string Name { get; set; }

    [StringLength(100)]
    public string? Description { get; set; }

    public ICollection<CourseClass> CourseClasses { get; set; } = [];
    public ICollection<StudentCourse> StudentCourses { get; set; } = [];
}
