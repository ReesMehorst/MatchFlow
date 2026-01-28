using FluentAssertions;
using MatchFlow.Api.Controllers;
using MatchFlow.Api.Dtos;
using MatchFlow.Domain.Entities;
using MatchFlow.Infrastructure.DBContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

public class TeamControllerTests
{
    private TeamController CreateController(
        MatchFlowDbContext db,
        string? userId = null,
        bool isAdmin = false)
    {
        var uploads = ControllerMocks.Uploads("/uploads/teams/logo.png");
        var currentUser = ControllerMocks.CurrentUser(userId);

        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.WebRootPath).Returns("wwwroot");
        env.Setup(e => e.ContentRootPath).Returns(".");

        var logger = new Mock<ILogger<TeamController>>();

        var controller = new TeamController(
            db,
            uploads.Object,
            currentUser.Object,
            env.Object,
            logger.Object
        );

        var claims = new List<Claim>();
        if (userId != null)
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        if (isAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };

        return controller;
    }

    // ---------- GET ----------

    [Fact]
    public async Task GetAll_ReturnsEmptyPagedResult_WhenNoTeams()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db);

        var result = await controller.GetAll(null, null, null, null, null);

        var ok = result.Result as OkObjectResult;
        var paged = ok!.Value as PagedResult<TeamListItemDto>;

        paged!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenMissing()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db);

        var result = await controller.Get(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    // ---------- CREATE ----------

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameMissing()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db, "user1");

        var dto = new CreateTeamDto { Tag = "ABC" };

        var result = await controller.Create(dto, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_CreatesTeam_AndAddsOwnerAsMember()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db, "owner1");

        var dto = new CreateTeamDto
        {
            Name = "My Team",
            Tag = "mt"
        };

        var result = await controller.Create(dto, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        db.Teams.Should().HaveCount(1);
        db.TeamMembers.Should().HaveCount(1);
        db.TeamMembers.First().Role.Should().Be("Owner");
    }

    // ---------- UPDATE ----------

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db, "u1");

        var dto = new UpdateTeamDto { Name = "X", Tag = "X" };

        var result = await controller.Update(Guid.NewGuid(), dto, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    // ---------- JOIN ----------

    [Fact]
    public async Task JoinTeam_ReturnsUnauthorized_WhenNoUser()
    {
        var db = TestDbFactory.Create();
        var team = new Team { Id = Guid.NewGuid(), Name = "T", Tag = "T", OwnerUserId = "o" };
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        var controller = CreateController(db, null);

        var result = await controller.JoinTeam(team.Id);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task JoinTeam_AddsMember_WhenValid()
    {
        var db = TestDbFactory.Create();
        var team = new Team { Id = Guid.NewGuid(), Name = "T", Tag = "T", OwnerUserId = "o" };
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        var controller = CreateController(db, "user1");

        var result = await controller.JoinTeam(team.Id);

        result.Should().BeOfType<OkResult>();
        db.TeamMembers.Should().HaveCount(1);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_ReturnsForbid_WhenNotOwnerOrAdmin()
    {
        var db = TestDbFactory.Create();
        var team = new Team { Id = Guid.NewGuid(), Name = "T", Tag = "T", OwnerUserId = "owner" };
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        var controller = CreateController(db, "other");

        var result = await controller.Delete(team.Id, CancellationToken.None);

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Delete_RemovesTeam_WhenOwner()
    {
        var db = TestDbFactory.Create();
        var team = new Team { Id = Guid.NewGuid(), Name = "T", Tag = "T", OwnerUserId = "owner" };
        db.Teams.Add(team);
        await db.SaveChangesAsync();

        var controller = CreateController(db, "owner");

        var result = await controller.Delete(team.Id, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        db.Teams.Should().BeEmpty();
    }
}