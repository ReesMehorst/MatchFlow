using FluentAssertions;
using MatchFlow.Api.Dtos;
using MatchFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

public class AuthenticationControllerTests
{
    private AuthenticationController CreateController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ClaimsPrincipal? user = null)
    {
        var controller = new AuthenticationController(
            userManager,
            signInManager,
            JwtTestConfiguration.Create()
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user ?? new ClaimsPrincipal()
            }
        };

        return controller;
    }

    // ---------------- REGISTER ----------------

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        await userManager.CreateAsync(new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test@test.com"
        });

        var controller = CreateController(userManager, signInManager);

        var result = await controller.Register(
            new RegisterDto("test@test.com", "Password123!", "Test")
        );

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_ReturnsToken_WhenSuccessful()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);
        var roleManager = IdentityTestFactory.CreateRoleManager(context);

        await roleManager.CreateAsync(new IdentityRole("User"));

        var controller = CreateController(userManager, signInManager);

        var result = await controller.Register(
            new RegisterDto("new@test.com", "Password123!", "New")
        );

        result.Value!.Token.Should().NotBeNullOrWhiteSpace();
    }

    // ---------------- LOGIN ----------------

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var controller = CreateController(userManager, signInManager);

        var result = await controller.Login(
            new LoginDto("missing@test.com", "password")
        );

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordInvalid()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test@test.com"
        };

        await userManager.CreateAsync(user, "CorrectPassword1!");

        var controller = CreateController(userManager, signInManager);

        var result = await controller.Login(
            new LoginDto("test@test.com", "WrongPassword")
        );

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsValid()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var user = new ApplicationUser
        {
            Id = "1",
            Email = "test@test.com",
            UserName = "test@test.com",
            DisplayName = "Test"
        };

        await userManager.CreateAsync(user, "Password123!");

        var controller = CreateController(userManager, signInManager);

        var result = await controller.Login(
            new LoginDto("test@test.com", "Password123!")
        );

        result.Value!.Token.Should().NotBeNullOrWhiteSpace();
    }

    // ---------------- ME ----------------

    [Fact]
    public async Task Me_ReturnsUnauthorized_WhenNoUserClaim()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var controller = CreateController(userManager, signInManager);

        var result = await controller.Me();

        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Me_ReturnsUnauthorized_WhenUserNotFound()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "missing")
        }));

        var controller = CreateController(userManager, signInManager, principal);

        var result = await controller.Me();

        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Me_ReturnsUserData_WhenAuthenticated()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var user = new ApplicationUser
        {
            Id = "123",
            Email = "me@test.com",
            UserName = "me@test.com",
            DisplayName = "Me"
        };

        await userManager.CreateAsync(user);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "123")
        }, "Test"));

        var controller = CreateController(userManager, signInManager, principal);

        var result = await controller.Me();

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var dto = ok!.Value as MeDto;
        dto!.Email.Should().Be("me@test.com");
    }

    // ---------------- DELETE USER ----------------

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var controller = CreateController(userManager, signInManager);

        var result = await controller.DeleteUser("missing");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteUser_ReturnsForbid_WhenCallerIsNotOwnerOrAdmin()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var user = new ApplicationUser
        {
            Id = "1",
            Email = "user@test.com",
            UserName = "user@test.com"
        };

        await userManager.CreateAsync(user);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "other")
        }));

        var controller = CreateController(userManager, signInManager, principal);

        var result = await controller.DeleteUser("1");

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenDeletingSelf()
    {
        var context = IdentityTestFactory.CreateDbContext();
        var userManager = IdentityTestFactory.CreateUserManager(context);
        var signInManager = IdentityTestFactory.CreateSignInManager(userManager);

        var user = new ApplicationUser
        {
            Id = "1",
            Email = "user@test.com",
            UserName = "user@test.com"
        };

        await userManager.CreateAsync(user);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "1")
        }));

        var controller = CreateController(userManager, signInManager, principal);

        var result = await controller.DeleteUser("1");

        result.Should().BeOfType<NoContentResult>();
    }
}