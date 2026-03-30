namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class StudentClassResponse
{
    public Guid ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset AssignedAt { get; set; }
    public string AssignedBy { get; set; } = string.Empty;
}
