namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class CreatedEntityResponse
{
    public Guid Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
