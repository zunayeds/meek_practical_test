using LearnWellUniversity_CMS.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace LearnWellUniversity_CMS.Infrastructure.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var idValue = httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            return Guid.TryParse(idValue, out var userId) ? userId : Guid.Empty;
        }
    }
}
