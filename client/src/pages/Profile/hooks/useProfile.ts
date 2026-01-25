import { useEffect, useState } from "react";
import { getProfile } from "../constants/profileConstants";
import type { UserProfile } from "../constants/profileConstants";

export function useProfile() {
    const [user, setUser] = useState<UserProfile | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getProfile()
            .then(setUser)
            .finally(() => setLoading(false));
    }, []);

    return { user, setUser, loading };
}