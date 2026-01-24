using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MatchFlow.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;
    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    public string? GetUserId()
    {
        var user = _http.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true) return null;
        // JWT may store the id in 'sub' or ClaimTypes.NameIdentifier
        return user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
