import { api } from "../../../services/api";

export type UserProfile = {
    id: string;
    displayName: string;
    email: string;
};

type UpdateProfilePayload = {
    displayName: string;
    email: string;
    password?: string;
};

export type Team = {
    id: string;
    name: string;
};

export const PROFILE_ENDPOINTS = {
    me: "/auth/me",
    update: "/auth/changedata",
    delete: "/users/me",
    joinedTeams: "/teams/joined"
};

export const getProfile = () =>
    api.get<UserProfile>("/auth/me");

export async function updateProfile(payload: UpdateProfilePayload): Promise<void> {
    await api.put("/auth/changedata", payload);
}

export const deleteProfile = () =>
    api.del("/auth/{id}");

export const getMyTeams = () =>
    api.get<Team[]>("/user/me/teams");