namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class StudentResponse : StudentResponseBase
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public Guid ModifiedBy { get; set; }
}
