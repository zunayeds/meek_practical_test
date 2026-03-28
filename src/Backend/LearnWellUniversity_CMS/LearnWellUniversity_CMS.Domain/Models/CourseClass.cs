using LearnWellUniversity_CMS.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Domain.Models;

public class CourseClass : AssignmentBase
{
    [Key]
    public Guid CourseClassId { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
}
