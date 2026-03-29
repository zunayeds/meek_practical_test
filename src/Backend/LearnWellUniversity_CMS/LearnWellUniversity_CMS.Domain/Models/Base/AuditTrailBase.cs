namespace LearnWellUniversity_CMS.Domain.Models.Base;

public interface IAuditTrailBase
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public ApplicationUser CreatedByUser { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public ApplicationUser? ModifiedByUser { get; set; }
}

public abstract class AuditTrailBase : IAuditTrailBase
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = null!;
    public DateTimeOffset? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public ApplicationUser? ModifiedByUser { get; set; }
}