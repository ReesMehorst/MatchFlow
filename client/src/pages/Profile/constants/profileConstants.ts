import { api } from "../../../services/api";

export type UserProfile = {
    id: string;
    displayName: string;
    email: string;
};

export type Team = {
    id: string;
    name: string;
};

export const PROFILE_ENDPOINTS = {
    me: "/auth/me",
    update: "/users/me",
    delete: "/users/me",
    joinedTeams: "/teams/joined"
};

export const getProfile = () =>
    api.get<UserProfile>("/auth/me");

export const updateProfile = (data: {
    displayName: string;
    email: string;
    password?: string;
}) =>
    api.put("/auth/me", data);

export const deleteProfile = () =>
    api.del("/auth/{id}");

export const getJoinedTeams = () =>
    api.get<Team[]>("/teams/joined");