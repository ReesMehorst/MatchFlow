import { api } from "./api";

export type AuthResult = {
    token: string;
    email: string;
    displayName: string;
    roles: string[];
};

export type RegisterDto = { email: string; password: string; displayName: string };
export type LoginDto = { email: string; password: string };

export const authApi = {
    register: (dto: RegisterDto) => api.post<AuthResult>("/auth/register", dto),
    login: (dto: LoginDto) => api.post<AuthResult>("/auth/login", dto),
};