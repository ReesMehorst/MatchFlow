using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;

namespace MatchFlow.Api.Dtos;

public record TeamDto(
    Guid Id, 
    string Name, 
    string Tag, 
    string OwnerUserId, 
    string? LogoUrl, 
    string? Bio, 
    DateTimeOffset CreatedAt
);

public sealed class CreateTeamDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = String.Empty;
    [Required, StringLength(5, MinimumLength = 2)] 
    public string Tag { get; set; } = String.Empty;
    public string? Bio { get; set; }
    [Required]
    public string? OwnerUserId { get; set; }
    public IFormFile? LogoFile { get; set; }
}

public sealed class UpdateTeamDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = String.Empty;
    [Required, StringLength(5, MinimumLength = 2)]
    public string Tag { get; set; } = String.Empty;
    public string? Bio { get; set; }
    public IFormFile? LogoFile { get; set; }
    public string? OwnerUserId { get; set; }
}