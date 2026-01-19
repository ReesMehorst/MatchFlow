using MatchFlow.Domain.Entities;
using MatchFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MatchFlow.Infrastructure.DBContext;

public sealed class MatchFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public MatchFlowDbContext(DbContextOptions<MatchFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<TeamGame> TeamGames => Set<TeamGame>();
    public DbSet<UserGame> UserGames => Set<UserGame>();
    public DbSet<Fixture> Fixtures => Set<Fixture>();
    public DbSet<FixtureTeam> FixtureTeams => Set<FixtureTeam>();
    public DbSet<FixtureParticipant> FixtureParticipants => Set<FixtureParticipant>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostMedia> PostMedia => Set<PostMedia>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Team unique constraints
        modelBuilder.Entity<Team>()
            .HasIndex(t => t.Name)
            .IsUnique();
        modelBuilder.Entity<Team>()
            .HasIndex(t => t.Tag)
            .IsUnique();

        // TeamMember: ensure single active membership per (Team,User)
        modelBuilder.Entity<TeamMember>()
            .HasIndex(tm => new { tm.TeamId, tm.UserId, tm.LeftAt });

        // TeamGame composite PK
        modelBuilder.Entity<TeamGame>()
            .HasKey(tg => new { tg.TeamId, tg.GameId });

        // UserGame composite PK
        modelBuilder.Entity<UserGame>()
            .HasKey(ug => new { ug.UserId, ug.GameId });

        // FixtureTeam composite PK and unique Side per Fixture
        modelBuilder.Entity<FixtureTeam>()
            .HasKey(ft => new { ft.FixtureId, ft.TeamId });
        modelBuilder.Entity<FixtureTeam>()
            .HasIndex(ft => new { ft.FixtureId, ft.Side })
            .IsUnique();

        // FixtureParticipant PK
        modelBuilder.Entity<FixtureParticipant>()
            .HasKey(fp => new { fp.FixtureId, fp.UserId });

        modelBuilder.Entity<PostLike>()
            .HasKey(pl => new { pl.PostId, pl.UserId, pl.TeamId });

        // Post: ensure XOR AuthorUserId/AuthorTeamId at app-level (SQL CHECK can be added if required)
        // Example: add a SQL CHECK if desired:
        // modelBuilder.Entity<Post>().HasCheckConstraint("CK_Post_AuthorXorTeam", "((AuthorUserId IS NULL) <> (AuthorTeamId IS NULL))");

        // map other relationships as needed (FKs set by conventions)
    }
}