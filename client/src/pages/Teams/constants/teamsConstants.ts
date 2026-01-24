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

function toQuery(filters: TeamFilters) {
    const p = new URLSearchParams();

    if (filters.search) p.set("search", filters.search);
    if (filters.tag) p.set("tag", filters.tag);
    if (filters.minMembers !== "" && filters.minMembers != null) p.set("minMembers", String(filters.minMembers));
    if (filters.maxMembers !== "" && filters.maxMembers != null) p.set("maxMembers", String(filters.maxMembers));
    if (filters.sort) p.set("sort", filters.sort);

    p.set("page", String(filters.page ?? 1));
    p.set("pageSize", String(filters.pageSize ?? 20));

    return p.toString();
}

export const teamsApi = {
    // Use compatibility endpoint for now to ensure the frontend receives the expected paged response
    list: (filters: TeamFilters) => api.get<TeamListResponse>(`/team/list`),
    join: (teamId: string) => api.post<void>(`/team/${teamId}/join`),
};