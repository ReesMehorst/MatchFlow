using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MatchFlow.Infrastructure.Identity;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterDto dto)
    {
        var existing = await _userManager.FindByNameAsync(dto.UserName);
        if (existing != null) return BadRequest(new AuthResultDto(false, null, new[] { "User already exists" }));

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(new AuthResultDto(false, null, result.Errors.Select(e => e.Description).ToArray()));

        var token = await GenerateJwtToken(user);
        return Ok(new AuthResultDto(true, token, Array.Empty<string>()));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login(LoginDto dto)
    {
        ApplicationUser? user = null;
        if (dto.UserNameOrEmail.Contains("@"))
            user = await _userManager.FindByEmailAsync(dto.UserNameOrEmail);
        else
            user = await _userManager.FindByNameAsync(dto.UserNameOrEmail);

        if (user is null) return Unauthorized(new AuthResultDto(false, null, new[] { "Invalid credentials" }));

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!signInResult.Succeeded) return Unauthorized(new AuthResultDto(false, null, new[] { "Invalid credentials" }));

        var token = await GenerateJwtToken(user);
        return Ok(new AuthResultDto(true, token, Array.Empty<string>()));
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
        var issuer = _config["Jwt:Issuer"] ?? "";
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim("displayName", user.DisplayName ?? string.Empty)
        };

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}