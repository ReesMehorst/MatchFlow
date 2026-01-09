using MatchFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MatchFlow.Infrastructure.Persistence;

public sealed class MatchFlowDbContext : DbContext
{
    public MatchFlowDbContext(DbContextOptions<MatchFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<GameMatch> Matches => Set<GameMatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Als je later entity configurations toevoegt:
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(MatchFlowDbContext).Assembly);
    }
}