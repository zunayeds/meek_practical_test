namespace LearnWellUniversity_CMS.Domain.Models.Base;

public abstract class AssignmentBase
{
    public DateTimeOffset AssignedAt { get; set; }
    public Guid AssignedBy { get; set; }
    public ApplicationUser AssignedByUser { get; set; } = null!;
}
