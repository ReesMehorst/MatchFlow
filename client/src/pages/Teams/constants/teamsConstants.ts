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

export const teamsApi = {
    // Fetch all teams (compatibility endpoint) and let client apply filters
    list: () => api.get<TeamListResponse>(`/team/list`),
    join: (teamId: string) => api.post<void>(`/team/${teamId}/join`),
};
