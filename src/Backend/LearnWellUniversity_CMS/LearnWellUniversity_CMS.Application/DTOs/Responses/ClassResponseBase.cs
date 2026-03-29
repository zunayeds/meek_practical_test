namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class ClassResponseBase
{
    public Guid ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
