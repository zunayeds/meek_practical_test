namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class AuthenticationResponse
{
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = [];
}
