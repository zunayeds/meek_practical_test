namespace LearnWellUniversity_CMS.Application.DTOs.Requests;

public class GetStudentByFiltersRequest : GetByFiltersBaseRequest
{
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
}
