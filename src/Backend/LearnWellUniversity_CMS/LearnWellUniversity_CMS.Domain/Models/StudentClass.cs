using LearnWellUniversity_CMS.Domain.Models.Base;

namespace LearnWellUniversity_CMS.Domain.Models;

public class StudentClass : AssignmentBase
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
}
