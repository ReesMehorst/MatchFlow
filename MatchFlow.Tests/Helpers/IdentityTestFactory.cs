using MatchFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

public static class IdentityTestFactory
{
    public static TestIdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestIdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestIdentityDbContext(options);
    }

    public static UserManager<ApplicationUser> CreateUserManager(
        TestIdentityDbContext context)
    {
        var store = new UserStore<ApplicationUser>(context);

        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());

        return new UserManager<ApplicationUser>(
            store,
            options.Object,
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object
        );
    }

    public static SignInManager<ApplicationUser> CreateSignInManager(
     UserManager<ApplicationUser> userManager)
    {
        return new SignInManager<ApplicationUser>(
            userManager,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null!,
            new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
            null!,
            null!
        );
    }

    public static RoleManager<IdentityRole> CreateRoleManager(
    TestIdentityDbContext context)
    {
        var store = new RoleStore<IdentityRole>(context);

        return new RoleManager<IdentityRole>(
            store,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object
        );
    }
}