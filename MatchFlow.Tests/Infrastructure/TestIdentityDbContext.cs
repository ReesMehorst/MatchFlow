using MatchFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class TestIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public TestIdentityDbContext(DbContextOptions options) : base(options)
    {
    }
}