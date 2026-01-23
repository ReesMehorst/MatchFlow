import React, { createContext, useEffect, useMemo, useState } from "react";
import type { LoginDto, RegisterDto } from "../services/authApi";
import { authApi } from "../services/authApi";
import { api } from "../services/api";

export type AuthUser = {
    id?: string;
    email: string;
    displayName: string;
    avatarUrl?: string | null;
    roles: string[];
    teamTag?: string | null;
    teamLogoUrl?: string | null;
};

export type AuthContextValue = {
    user: AuthUser | null;
    isAuthenticated: boolean;
    login: (dto: LoginDto) => Promise<void>;
    register: (dto: RegisterDto) => Promise<void>;
    logout: () => void;
};

export const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function loadUser(): AuthUser | null {
    const raw = localStorage.getItem("mf_user");
    return raw ? (JSON.parse(raw) as AuthUser) : null;
}

function parseJwt(token: string) {
    try {
        const payload = token.split(".")[1];
        const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
        const json = decodeURIComponent(
            atob(base64)
                .split("")
                .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
                .join("")
        );
        return JSON.parse(json);
    } catch {
        return null;
    }
}

async function enrichUserFromToken(u: AuthUser, token: string): Promise<AuthUser> {
    try {
        const jwt = parseJwt(token);
        const id = jwt?.sub as string | undefined;
        const out = { ...u, id };

        if (id) {
            // try to find a team where OwnerUserId matches user id
            // TeamDto shape: { id, name, tag, ownerUserId, logoUrl, bio, createdAt }
            const teams = await api.get<any[]>("/team");
            const primary = teams.find((t) => t.ownerUserId === id);
            if (primary) {
                out.teamTag = primary.tag ?? null;
                out.teamLogoUrl = primary.logoUrl ?? null;
            }
        }

        return out;
    } catch {
        return u;
    }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<AuthUser | null>(() => loadUser());

    // On mount, if we have a token and user but missing id/team info, attempt to enrich
    useEffect(() => {
        (async () => {
            const token = localStorage.getItem("mf_token");
            const saved = loadUser();
            if (token && saved && (!saved.id || !saved.teamTag)) {
                const enriched = await enrichUserFromToken(saved, token);
                localStorage.setItem("mf_user", JSON.stringify(enriched));
                setUser(enriched);
            }
        })();
    }, []);

    const login: AuthContextValue["login"] = async (dto) => {
        const res = await authApi.login(dto);
        localStorage.setItem("mf_token", res.token);
        const baseUser: AuthUser = {
            email: res.email,
            displayName: res.displayName,
            roles: res.roles,
            avatarUrl: null,
        };
        const enriched = await enrichUserFromToken(baseUser, res.token);
        localStorage.setItem("mf_user", JSON.stringify(enriched));
        setUser(enriched);
    };

    const register: AuthContextValue["register"] = async (dto) => {
        const res = await authApi.register(dto);
        localStorage.setItem("mf_token", res.token);
        const baseUser: AuthUser = {
            email: res.email,
            displayName: res.displayName,
            roles: res.roles,
            avatarUrl: null,
        };
        const enriched = await enrichUserFromToken(baseUser, res.token);
        localStorage.setItem("mf_user", JSON.stringify(enriched));
        setUser(enriched);
    };

    const logout = () => {
        localStorage.removeItem("mf_token");
        localStorage.removeItem("mf_user");
        setUser(null);
    };

    const value = useMemo(
        () => ({ user, isAuthenticated: !!user, login, register, logout }),
        [user]
    );

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}