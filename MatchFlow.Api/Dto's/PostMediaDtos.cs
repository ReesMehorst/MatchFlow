using MediaType = MatchFlow.Domain.Entities.MediaType;
using System;

namespace MatchFlow.Api.Dtos;

public record PostMediaDto(
    Guid Id, 
    Guid PostId, 
    string Url, 
    string? ThumbnailUrl, 
    MediaType MediaType, 
    int? DurationSec, 
    DateTimeOffset CreatedAt
);

public record CreatePostMediaDto(
    Guid PostId, 
    string Url, 
    string? ThumbnailUrl, 
    MediaType MediaType, 
    int? DurationSec
);

public record UpdatePostMediaDto(
    string? ThumbnailUrl, 
    int? DurationSec
);