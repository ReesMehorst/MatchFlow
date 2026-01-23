using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MatchFlow.Infrastructure.Identity;
using MatchFlow.Api.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null) return BadRequest("Email already in use.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.Select(e => e.Description));

        // Optional: default role
        await _userManager.AddToRoleAsync(user, "User");

        return await IssueToken(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login(LoginDto dto)
    {
        var identifier = (dto.Email ?? string.Empty).Trim();

        // Try common identifier types: email, username, displayName (case-insensitive)
        ApplicationUser? user = null;

        if (!string.IsNullOrEmpty(identifier))
        {
            user = await _userManager.FindByEmailAsync(identifier);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(identifier);
            }

            if (user == null)
            {
                // search by DisplayName (case-insensitive)
                var lowered = identifier.ToLower();
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.DisplayName != null && u.DisplayName.ToLower() == lowered);
            }
        }

        if (user == null) return Unauthorized("Invalid credentials.");

        var ok = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!ok.Succeeded) return Unauthorized("Invalid credentials.");

        return await IssueToken(user);
    }

    private async Task<AuthResultDto> IssueToken(ApplicationUser user)
    {
        var jwt = _config.GetSection("Jwt");
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new("displayName", user.DisplayName ?? ""),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResultDto(
            tokenString,
            user.Email ?? "",
            user.DisplayName ?? "",
            roles.ToArray()
        );
    }
}