const API_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7267/api";

function getToken(): string | null {
    return localStorage.getItem("mf_token");
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
    const headers = new Headers(options.headers);
    headers.set("Content-Type", "application/json");

    const token = getToken();
    if (token) headers.set("Authorization", `Bearer ${token}`);

    const res = await fetch(`${API_URL}${path}`, { ...options, headers });
    const text = await res.text();
    const data = text ? JSON.parse(text) : null;

    if (!res.ok) {
        const message = typeof data === "string" ? data : data?.message ?? "Request failed";
        throw new Error(message);
    }

    return data as T;
}

export const api = {
    get: <T>(path: string) => request<T>(path),
    post: <T>(path: string, body?: unknown) =>
        request<T>(path, { method: "POST", body: JSON.stringify(body ?? {}) }),
};