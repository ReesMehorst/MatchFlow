import { useEffect, useMemo } from "react";

export function useImagePreview(file: File | null) {
    // Create object URL synchroon zodat setState niet nodig is.
    const url = useMemo(() => (file ? URL.createObjectURL(file) : null), [file]);

    // revoked wanneer component unmount of file verandert
    useEffect(() => {
        return () => {
            if (url) URL.revokeObjectURL(url);
        };
    }, [url]);

    return url;
}