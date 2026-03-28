namespace LearnWellUniversity_CMS.Application.DTOs.Responses;

public class AuthenticationResponse
{
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = [];
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
