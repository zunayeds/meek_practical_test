using LearnWellUniversity_CMS.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Domain.Models;

public class StudentCourse : AssignmentBase
{
    [Key]
    public Guid StudentCourseId { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
