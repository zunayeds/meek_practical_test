using LearnWellUniversity_CMS.Domain.Models.Base;

namespace LearnWellUniversity_CMS.Domain.Models;

public class StudentCourse : AssignmentBase
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
