using FluentAssertions;
using MatchFlow.Api.Controllers;
using MatchFlow.Api.Dtos;
using MatchFlow.Domain.Entities;
using MatchFlow.Infrastructure.DBContext;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class TeamMemberControllerTests
{
    private TeamMemberController CreateController(MatchFlowDbContext db)
    {
        return new TeamMemberController(db);
    }

    // ---------- GET ALL ----------

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoMembers()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db);

        var result = await controller.GetAll();

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var list = ok!.Value as IEnumerable<TeamMemberDto>;
        list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnsMembers_WhenPresent()
    {
        var db = TestDbFactory.Create();

        db.TeamMembers.Add(new TeamMember
        {
            TeamId = Guid.NewGuid(),
            UserId = "user1",
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync();

        var controller = CreateController(db);

        var result = await controller.GetAll();

        var ok = result.Result as OkObjectResult;
        var list = ok!.Value as IEnumerable<TeamMemberDto>;

        list.Should().HaveCount(1);
    }

    // ---------- GET ----------

    [Fact]
    public async Task Get_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db);

        var result = await controller.Get(Guid.NewGuid(), "missing");

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Get_ReturnsMember_WhenExists()
    {
        var db = TestDbFactory.Create();

        var teamId = Guid.NewGuid();
        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = "user1",
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        };

        db.TeamMembers.Add(member);
        await db.SaveChangesAsync();

        var controller = CreateController(db);

        var result = await controller.Get(teamId, "user1");

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var dto = ok!.Value as TeamMemberDto;
        dto!.UserId.Should().Be("user1");
    }

    // ---------- CREATE ----------

    [Fact]
    public async Task Create_ReturnsConflict_WhenMemberAlreadyExists()
    {
        var db = TestDbFactory.Create();

        var teamId = Guid.NewGuid();
        db.TeamMembers.Add(new TeamMember
        {
            TeamId = teamId,
            UserId = "user1",
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync();

        var controller = CreateController(db);

        var dto = new CreateTeamMemberDto(
            teamId,
            "user1",
            "Member"
        );

        var result = await controller.Create(dto);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Create_CreatesMember_WhenNotExisting()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db);

        var teamId = Guid.NewGuid();

        var dto = new CreateTeamMemberDto(
            teamId,
            "user1",
            "Member"
        );

        var result = await controller.Create(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        db.TeamMembers.Should().HaveCount(1);
    }

    // ---------- UPDATE ----------

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db);

        var dto = new UpdateTeamMemberDto(
            "Admin",
            null
        );

        var result = await controller.Update(Guid.NewGuid(), "missing", dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_UpdatesRoleAndLeftAt_WhenExists()
    {
        var db = TestDbFactory.Create();

        var teamId = Guid.NewGuid();
        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = "user1",
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        };

        db.TeamMembers.Add(member);
        await db.SaveChangesAsync();

        var controller = CreateController(db);

        var leftAt = DateTimeOffset.UtcNow;

        var dto = new UpdateTeamMemberDto(
            "Owner",
            leftAt
        );

        var result = await controller.Update(teamId, "user1", dto);

        result.Should().BeOfType<NoContentResult>();

        var updated = await db.TeamMembers.FindAsync(teamId, "user1");
        updated!.Role.Should().Be("Owner");
        updated.LeftAt.Should().Be(leftAt);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMemberDoesNotExist()
    {
        var db = TestDbFactory.Create();
        var controller = CreateController(db);

        var result = await controller.Delete(Guid.NewGuid(), "missing");

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_RemovesMember_WhenExists()
    {
        var db = TestDbFactory.Create();

        var teamId = Guid.NewGuid();
        db.TeamMembers.Add(new TeamMember
        {
            TeamId = teamId,
            UserId = "user1",
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync();

        var controller = CreateController(db);

        var result = await controller.Delete(teamId, "user1");

        result.Should().BeOfType<NoContentResult>();
        db.TeamMembers.Should().BeEmpty();
    }
}