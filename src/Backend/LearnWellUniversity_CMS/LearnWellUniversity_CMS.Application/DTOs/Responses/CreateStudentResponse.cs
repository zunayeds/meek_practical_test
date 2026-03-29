namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class CreateStudentResponse : CreatedEntityResponse
{
    public string Password { get; set; } = string.Empty;
}
