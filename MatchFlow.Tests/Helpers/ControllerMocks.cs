using MatchFlow.Api.Services;
using Microsoft.AspNetCore.Http;
using Moq;

public static class ControllerMocks
{
    public static Mock<ICurrentUserService> CurrentUser(string? userId)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.GetUserId()).Returns(userId);
        return mock;
    }

    public static Mock<IUploadService> Uploads(string? returnUrl = null)
    {
        var mock = new Mock<IUploadService>();
        mock.Setup(x => x.SaveTeamLogoAsync(It.IsAny<IFormFile?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnUrl);
        return mock;
    }
}