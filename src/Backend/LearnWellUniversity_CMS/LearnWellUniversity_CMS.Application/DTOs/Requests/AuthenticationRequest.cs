namespace LearnWellUniversity_CMS.Application.DTOs.Requests
{
    public class AuthenticationRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
