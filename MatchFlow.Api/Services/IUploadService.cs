using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MatchFlow.Api.Services;

public interface IUploadService
{
    /// <summary>
    /// Saves a team logo to the webroot uploads folder and returns the relative path ("/uploads/teams/{file}") or null if no file provided.
    /// Throws ArgumentException for invalid files.
    /// </summary>
    Task<string?> SaveTeamLogoAsync(IFormFile? file, CancellationToken cancellationToken = default);
}
