using LearnWellUniversity_CMS.Domain.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnWellUniversity_CMS.Domain.Models;

public class Student : AuditTrailBase
{
    [Key]
    public Guid StudentId { get; set; }

    [StringLength(50)]
    [Column(TypeName = "citext")]
    public required string FirstName { get; set; }

    [StringLength(50)]
    [Column(TypeName = "citext")]
    public required string LastName { get; set; }

    [EmailAddress]
    [StringLength(50)]
    public required string EmailAddress { get; set; }

    [Phone]
    [StringLength(20)]
    public required string PhoneNumber { get; set; }

    [StringLength(200)]
    public required string Address { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public ICollection<StudentCourse> StudentCourses { get; set; } = [];
    public ICollection<StudentClass> StudentClasses { get; set; } = [];
}
