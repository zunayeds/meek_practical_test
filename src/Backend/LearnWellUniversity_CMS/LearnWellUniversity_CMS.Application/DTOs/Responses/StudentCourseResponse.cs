namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class StudentCourseResponse
{
    public Guid CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset AssignedAt { get; set; }
    public string AssignedBy { get; set; } = string.Empty;
}
