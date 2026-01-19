using System;

namespace MatchFlow.Api.Dtos;

public record TeamMemberDto(Guid Id, Guid TeamId, string UserId, string Role, DateTimeOffset JoinedAt, DateTimeOffset? LeftAt);
public record CreateTeamMemberDto(Guid TeamId, string UserId, string Role);
public record UpdateTeamMemberDto(string Role, DateTimeOffset? LeftAt);