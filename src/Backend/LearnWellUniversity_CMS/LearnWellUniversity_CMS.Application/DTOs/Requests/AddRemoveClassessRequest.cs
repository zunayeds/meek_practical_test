namespace LearnWellUniversity_CMS.Application.DTOs.Requests;

public class AddRemoveClassessRequest
{
    public required List<Guid> AddClassIds { get; set; } = new List<Guid>();
    public required List<Guid> RemoveClassIds { get; set; } = new List<Guid>();
}
