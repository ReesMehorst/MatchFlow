import { api, API_URL } from "../../../services/api";

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

export async function deleteProfile(password: string) {
    const res = await fetch(`${API_URL}/auth/me`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("mf_token")}`,
        },
        body: JSON.stringify({ password }),
    });

    if (!res.ok) {
        throw new Error("Delete failed");
    }
}

export const getMyTeams = () =>
    api.get<Team[]>("/user/me/teams");