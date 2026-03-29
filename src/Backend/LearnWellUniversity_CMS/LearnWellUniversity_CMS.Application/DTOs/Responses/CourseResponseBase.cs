namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class CourseResponseBase
{
    public Guid CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
