using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MatchFlow.Api.Dtos;
using MatchFlow.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration config
) : ControllerBase
{
    private static readonly string[] UserAlreadyExistsErrors = ["User already exists"];
    private static readonly string[] InvalidCredentialsErrors = ["Invalid credentials"];

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register(RegisterDto dto)
    {
        var existing = await userManager.FindByNameAsync(dto.UserName);
        if (existing is not null)
            return BadRequest(new AuthResultDto(false, null, UserAlreadyExistsErrors));

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new AuthResultDto(
                false,
                null,
                result.Errors.Select(e => e.Description).ToArray()
            ));

        var token = await GenerateJwtToken(user);
        return Ok(new AuthResultDto(true, token, Array.Empty<string>()));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login(LoginDto dto)
    {
        var user = dto.UserNameOrEmail.Contains('@')
            ? await userManager.FindByEmailAsync(dto.UserNameOrEmail)
            : await userManager.FindByNameAsync(dto.UserNameOrEmail);

        if (user is null)
            return Unauthorized(new AuthResultDto(false, null, InvalidCredentialsErrors));

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: false
        );

        if (!signInResult.Succeeded)
            return Unauthorized(new AuthResultDto(false, null, InvalidCredentialsErrors));

        var token = await GenerateJwtToken(user);
        return Ok(new AuthResultDto(true, token, Array.Empty<string>()));
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var key = config["Jwt:Key"]
                  ?? Environment.GetEnvironmentVariable("JWT_KEY")
                  ?? throw new InvalidOperationException(
                      "Jwt:Key not configured. Add Jwt:Key to appsettings or set JWT_KEY environment variable."
                  );

        var issuer = config["Jwt:Issuer"]
                     ?? Environment.GetEnvironmentVariable("JWT_ISSUER")
                     ?? "MatchFlow";

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new("displayName", user.DisplayName ?? string.Empty)
        ];

        claims.AddRange(await userManager.GetClaimsAsync(user));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}