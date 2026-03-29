using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Shared.Constants;
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

    public Guid? StudentId
    {
        get
        {
            var idClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(w => w.Type == AdditionalClaims.StudentId);
            if (idClaim is null) return null;
            return Guid.TryParse(idClaim?.Value, out var studentId) ? studentId : Guid.Empty;
        }
    }
}
