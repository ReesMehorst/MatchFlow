import { api } from "../../../services/api";

export type TeamSummary = {
    id: string;
    name: string;
    tag: string;
    memberCount: number;
    bio?: string | null;
    logoUrl?: string | null;
    isMember: boolean;
};

export type TeamListResponse = {
    items: TeamSummary[];
    page: number;
    pageSize: number;
    total: number;
};

export type TeamFilters = {
    search?: string;
    tag?: string;
    minMembers?: number | "";
    maxMembers?: number | "";
    sort?: "created_desc" | "name_asc" | "members_desc" | "members_asc";
    page?: number;
    pageSize?: number;
};

export type TeamDto = {
  id: string;
  name: string;
  tag: string;
  ownerUserId: string;
  logoUrl?: string | null;
  bio?: string | null;
  createdAt: string;
};

export type TeamMemberDto = {
  id: string;
  teamId: string;
  userId: string;
  role: string;
  joinedAt: string;
  leftAt?: string | null;
};

export type TeamMatch = {
    id: string;
    teamAId: string;
    teamBId: string;
    date: string;
    location?: string | null;
    scoreA?: number | null;
    scoreB?: number | null;
};

export const teamsApi = {
    //Alle endpoints voor teams
    list: () => api.get<TeamListResponse>(`/team/list`),
    join: (teamId: string) => api.post<void>(`/team/${teamId}/join`),
    get: (teamId: string) => api.get<TeamDto>(`/team/${teamId}`),
    getMembers: () => api.get<TeamMemberDto[]>(`/teamMember`),
    getMatches: (teamId: string) => api.get<TeamMatch[]>(`/fixture?teamId=${teamId}`),
};
