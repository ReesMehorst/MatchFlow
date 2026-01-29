// Allow an explicit runtime override via globalThis.__MF_API_URL (useful during dev)
// Fallback to the local API address used by the ASP.NET project.
export let API_URL = (globalThis as any).__MF_API_URL as string | undefined;
API_URL = API_URL ?? "https://localhost:5001/api";
if (!API_URL.endsWith("/api")) {
    // allow both forms: https://host and https://host/api -> normalize to include /api
    if (API_URL.endsWith("/")) API_URL = API_URL + "api";
    else API_URL = API_URL + "/api";
}

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

    // Try fetch and on network error attempt a non-https localhost fallback
    let res: Response;
    try {
        res = await fetch(`${API_URL}${path}`, fetchOptions);
    } catch (err) {
        // If the configured API is https localhost try http fallback (common dev setup)
        if (API_URL.startsWith("https://localhost")) {
            try {
                const fallback = API_URL.replace("https://", "http://");
                res = await fetch(`${fallback}${path}`, fetchOptions);
            } catch (err2) {
                throw new Error(
                    `Network error while contacting API (tried ${API_URL} and fallback): ${
                        (err as Error)?.message ?? String(err)
                    }`
                );
            }
        } else {
            throw new Error(`Network error while contacting API: ${(err as Error)?.message ?? String(err)}`);
        }
    }

    const text = await res.text();
    const data = text ? tryParseJson(text) : null;

    if (!res.ok) {
        // If the server returned a plain string payload use that
        if (typeof data === "string") {
            // Avoid showing raw HTML error pages to the user
            const asStr = data.trim();
            if (asStr.startsWith("<")) {
                throw new Error(`Request failed: ${res.status} ${res.statusText}`);
            }
            throw new Error(asStr);
        }

        // Handle typical ASP.NET ProblemDetails / validation shapes
        if (isRecord(data)) {
            const candidates = ["message", "title", "detail", "error"];
            for (const key of candidates) {
                const v = data[key];
                if (typeof v === "string" && v.length > 0) {
                    throw new Error(v);
                }
            }

            // Validation errors often come under `errors` as an object
            const maybeErrors = data["errors"];
            if (isRecord(maybeErrors)) {
                const parts: string[] = [];
                for (const k of Object.keys(maybeErrors)) {
                    const val = maybeErrors[k];
                    if (Array.isArray(val)) parts.push(`${k}: ${val.join(", ")}`);
                    else if (typeof val === "string") parts.push(`${k}: ${val}`);
                }
                if (parts.length) throw new Error(parts.join("; "));
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