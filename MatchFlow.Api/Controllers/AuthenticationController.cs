using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<MeDto>> Me()
    {
        // UserId is stored in the JWT as sub
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new MeDto(
            user.Id,
            user.Email ?? "",
            user.DisplayName,
            roles.ToArray()
        ));
    }

    [Authorize]
    [HttpPut("changedata")]
    public async Task<IActionResult> ChangeData(ChangeUserDataDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        user.DisplayName = dto.DisplayName;

        if (user.Email != dto.Email)
        {
            var emailResult = await _userManager.SetEmailAsync(user, dto.Email);
            if (!emailResult.Succeeded)
                return BadRequest(emailResult.Errors);

            var usernameResult = await _userManager.SetUserNameAsync(user, dto.Email);
            if (!usernameResult.Succeeded)
                return BadRequest(usernameResult.Errors);
        }

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(
                user,
                token,
                dto.Password
            );

            if (!passwordResult.Succeeded)
                return BadRequest(passwordResult.Errors);
        }

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe([FromBody] DeleteAccountDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Password is required");

        var userId =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid)
            return BadRequest("Invalid password");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return NoContent();
    }
}