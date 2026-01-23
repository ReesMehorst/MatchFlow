export function normalizeTeamTag(input: string) {
    // Keep existing behavior: no uppercasing; tag is case sensitive.
    return input.slice(0, 5);
}

export function validateTeamTag(tag: string) {
    const t = tag.trim();
    if (t.length === 0) return null;
    if (t.length < 2) return "Tag must be at least 2 characters.";
    if (t.length > 5) return "Tag must be at most 5 characters.";
    return null;
}

export function validateLogoFile(file: File) {
    // Accept image/png, image/jpeg, image/jpg
    if (!/^image\/(png|jpeg|jpg)$/.test(file.type)) {
        return "Logo must be a PNG or JPEG image.";
    }
    if (file.size > 2 * 1024 * 1024) {
        return "Logo must be smaller than 2MB.";
    }
    return null;
}

// If your api wrapper sometimes returns { data: ... } and sometimes the data directly.
export function unwrapApiData<T>(value: unknown): T {
    if (value && typeof value === "object" && "data" in (value as Record<string, unknown>)) {
        return (value as { data: T }).data;
    }
    return value as T;
}