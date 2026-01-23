const API_URL = import.meta.env.VITE_API_URL ?? "https://localhost:5001/api";

function getToken(): string | null {
    return localStorage.getItem("mf_token");
}

type RequestOptions = Omit<RequestInit, "body" | "headers"> & {
    headers?: HeadersInit;
    body?: unknown;
};

function isRecord(v: unknown): v is Record<string, unknown> {
    return typeof v === "object" && v !== null;
}

function isBodyInit(v: unknown): v is BodyInit {
    if (typeof v === "string") return true;
    if (typeof Blob !== "undefined" && v instanceof Blob) return true;
    if (typeof FormData !== "undefined" && v instanceof FormData) return true;
    if (typeof URLSearchParams !== "undefined" && v instanceof URLSearchParams) return true;
    if (typeof ArrayBuffer !== "undefined" && v instanceof ArrayBuffer) return true;
    if (typeof ArrayBuffer !== "undefined" && ArrayBuffer.isView(v)) return true;
    if (typeof ReadableStream !== "undefined" && v instanceof ReadableStream) return true;
    return false;
}

function tryParseJson(text: string): unknown {
    try {
        const parsed: unknown = JSON.parse(text) as unknown;
        return parsed;
    } catch {
        return text;
    }
}

async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
    const { body: rawBody, headers: headerInit, ...rest } = options;

    const headers = new Headers(headerInit);

    const token = getToken();
    if (token) headers.set("Authorization", `Bearer ${token}`);

    let body: BodyInit | undefined;

    if (rawBody !== undefined && rawBody !== null) {
        if (isBodyInit(rawBody)) {
            body = rawBody; // narrowed to BodyInit
        } else {
            headers.set("Content-Type", "application/json");
            body = JSON.stringify(rawBody);
        }
    }

    const fetchOptions: RequestInit = {
        ...rest,
        headers,
        ...(body !== undefined ? { body } : {}),
    };

    const res = await fetch(`${API_URL}${path}`, fetchOptions);

    const text = await res.text();
    const data = text ? tryParseJson(text) : null;

    if (!res.ok) {
        // Prefer explicit, typed checks over `any`.
        if (typeof data === "string") {
            throw new Error(data);
        }

        if (isRecord(data)) {
            const maybeMessage = data["message"];
            if (typeof maybeMessage === "string") {
                throw new Error(maybeMessage);
            }
        }

        throw new Error("Request failed");
    }

    return data as T;
}

export const api = {
    get: <T>(path: string, options?: RequestOptions) => request<T>(path, options),
    post: <T>(path: string, body?: unknown, options?: RequestOptions) =>
        request<T>(path, { method: "POST", body, ...(options ?? {}) }),
    put: <T>(path: string, body?: unknown, options?: RequestOptions) =>
        request<T>(path, { method: "PUT", body, ...(options ?? {}) }),
    del: <T>(path: string, options?: RequestOptions) =>
        request<T>(path, { method: "DELETE", ...(options ?? {}) }),
};