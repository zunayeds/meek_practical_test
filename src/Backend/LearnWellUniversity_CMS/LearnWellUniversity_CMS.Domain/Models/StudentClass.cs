using LearnWellUniversity_CMS.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Domain.Models;

public class StudentClass : AssignmentBase
{
    [Key]
    public Guid StudentClassId { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
}
