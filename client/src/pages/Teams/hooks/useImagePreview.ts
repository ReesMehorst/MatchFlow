import { useEffect, useMemo } from "react";

export function useImagePreview(file: File | null) {
    // Create object URL synchronously in useMemo so no setState is required in an effect.
    const url = useMemo(() => (file ? URL.createObjectURL(file) : null), [file]);

    // Revoke when the URL changes / on unmount.
    useEffect(() => {
        return () => {
            if (url) URL.revokeObjectURL(url);
        };
    }, [url]);

    return url;
}