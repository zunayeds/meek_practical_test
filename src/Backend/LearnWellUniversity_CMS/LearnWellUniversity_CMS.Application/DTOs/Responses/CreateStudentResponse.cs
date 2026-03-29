namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class CreateStudentResponse : EntityCreatedResponse
{
    public string Password { get; set; } = string.Empty;
}
