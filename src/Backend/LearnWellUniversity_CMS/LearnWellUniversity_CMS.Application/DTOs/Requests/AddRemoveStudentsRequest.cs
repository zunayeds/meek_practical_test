namespace LearnWellUniversity_CMS.Application.DTOs.Requests;

public class AddRemoveStudentsRequest
{
    public required List<Guid> AddStudentIds { get; set; } = new List<Guid>();
    public required List<Guid> RemoveStudentIds { get; set; } = new List<Guid>();
}
