using LearnWellUniversity_CMS.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Domain.Models;

public class Class : AuditTrailBase
{
    [Key]
    public Guid ClassId { get; set; }

    [StringLength(20)]
    public required string Name { get; set; }

    [StringLength(100)]
    public string? Description { get; set; }

    public ICollection<CourseClass> CourseClasses { get; set; } = [];
    public ICollection<StudentClass> StudentClasses { get; set; } = [];
}
