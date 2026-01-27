using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MatchFlow.Infrastructure.Identity;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Api.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public UserController(MatchFlowDbContext db)
    {
        _db = db;
    }

    [HttpGet("me/teams")]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetMyTeams()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var teams = await _db.TeamMembers
            .Where(tm => tm.UserId == userId)
            .Select(tm => new TeamDto(
                tm.Team.Id,
                tm.Team.Name,
                tm.Team.Tag,
                tm.Team.OwnerUserId,
                tm.Team.LogoUrl,
                tm.Team.Bio,
                tm.Team.CreatedAt
            ))
            .ToListAsync();

        return Ok(teams);
    }
}
