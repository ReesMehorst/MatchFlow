namespace MatchFlow.Api.Dtos;

public record RegisterDto(string UserName, string DisplayName, string Email, string Password);
public record LoginDto(string UserNameOrEmail, string Password);
public record AuthResultDto(bool Success, string? Token, string[] Errors);