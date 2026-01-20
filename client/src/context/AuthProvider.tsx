import React, { createContext, useMemo, useState } from "react";
import type { LoginDto, RegisterDto } from "../services/authApi";
import { authApi } from "../services/authApi";

export type AuthUser = {
    email: string;
    displayName: string;
    roles: string[];
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

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<AuthUser | null>(() => loadUser());

    const login: AuthContextValue["login"] = async (dto) => {
        const res = await authApi.login(dto);
        localStorage.setItem("mf_token", res.token);
        const u: AuthUser = { email: res.email, displayName: res.displayName, roles: res.roles };
        localStorage.setItem("mf_user", JSON.stringify(u));
        setUser(u);
    };

    const register: AuthContextValue["register"] = async (dto) => {
        const res = await authApi.register(dto);
        localStorage.setItem("mf_token", res.token);
        const u: AuthUser = { email: res.email, displayName: res.displayName, roles: res.roles };
        localStorage.setItem("mf_user", JSON.stringify(u));
        setUser(u);
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