using LearnWellUniversity_CMS.Domain.Models.Base;

namespace LearnWellUniversity_CMS.Domain.Models;

public class CourseClass : AssignmentBase
{
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
}
