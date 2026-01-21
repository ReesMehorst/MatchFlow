using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Infrastructure.Identity;
using System.Text;
using MatchFlow.Domain.Entities;
using Microsoft.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Identity.UI;
using System.Linq;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<MatchFlowDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Identity
IdentityBuilder identityBuilder = builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 8;
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<MatchFlowDbContext>()
.AddDefaultTokenProviders();

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// CORS for Vite dev server
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("client", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MatchFlow API", Version = "v1" });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// add after `var app = builder.Build();`
app.UseStaticFiles();

// ensure uploads folder exists
var uploads = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads", "teams");
Directory.CreateDirectory(uploads);

// then map controllers as usual
app.MapControllers();

// Ensure Identity roles exist (seed roles) to avoid AddToRoleAsync throwing when role missing
try
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RoleSeeder");

    var rolesToEnsure = new[] { "User", "Admin" };
    foreach (var roleName in rolesToEnsure)
    {
        var exists = roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
        if (!exists)
        {
            var createResult = roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to create role {Role}: {Errors}", roleName,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
            else
            {
                logger.LogInformation("Created missing role {Role}", roleName);
            }
        }
    }
}
catch (Exception ex)
{
    var lf = app.Services.GetRequiredService<ILoggerFactory>();
    var logger = lf.CreateLogger("RoleSeeder");
    logger.LogError(ex, "Error while seeding roles");
}

if (app.Environment.IsDevelopment())
{
    // Serve swagger and make it the app root in dev so the browser opens to it
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MatchFlow API v1");
        // If you want the UI at / instead of /swagger set RoutePrefix = string.Empty
        // c.RoutePrefix = string.Empty;
    });
}
else
{
    // In production serve the built SPA from wwwroot
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
}

app.UseHttpsRedirection();

app.UseCors("client");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();