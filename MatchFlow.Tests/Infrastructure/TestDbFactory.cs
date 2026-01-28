using MatchFlow.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

public static class TeamTestDbFactory
{
    public static MatchFlowDbContext Create()
    {
        var options = new DbContextOptionsBuilder<MatchFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MatchFlowDbContext(options);
    }
}