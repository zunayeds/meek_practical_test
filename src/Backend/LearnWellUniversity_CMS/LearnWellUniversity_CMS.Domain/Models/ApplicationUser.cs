using LearnWellUniversity_CMS.Domain.Models.Base;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LearnWellUniversity_CMS.Domain.Models;

public class ApplicationUser : IdentityUser<Guid>, IAuditTrailBase
{
    [StringLength(50)]
    public required string FirstName { get; set; }

    [StringLength(50)]
    public required string LastName { get; set; }

    // Audit Trail Properties
    public DateTimeOffset CreatedAt { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = null!;
    public Guid CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public ApplicationUser? ModifiedByUser { get; set; }
    public bool IsActive { get; set; }
}
