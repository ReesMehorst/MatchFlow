import { useEffect, useMemo, useState } from "react";
import { teamsApi, type TeamFilters, type TeamListResponse } from "../constants/teamsConstants";

export function useTeams(initial?: TeamFilters) {
    const [filters, setFiltersState] = useState<TeamFilters>({
        search: "",
        tag: "",
        minMembers: "",
        maxMembers: "",
        sort: "members_desc",
        page: 1,
        pageSize: 20,
        ...initial,
    });

    const [data, setData] = useState<TeamListResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const setFilters = (next: TeamFilters) => {
        setFiltersState((prev) => ({
            ...prev,
            ...next,
            page: next.page ?? 1,
        }));
    };

    const stableFilters = useMemo(() => filters, [filters]);

    useEffect(() => {
        let cancelled = false;

        (async () => {
            setLoading(true);
            setError(null);
            try {
                const res = await teamsApi.list(stableFilters);
                console.log('[useTeams] fetched teams', { filters: stableFilters, res });
                if (!cancelled) setData(res);
            } catch (e) {
            if (!cancelled) {
                console.error('[useTeams] error fetching teams', e);
                setError(e instanceof Error ? e.message : "Failed to load teams");
            }
            } finally {
                if (!cancelled) setLoading(false);
            }
        })();

        return () => {
            cancelled = true;
        };
    }, [stableFilters]);

    const joinTeam = async (teamId: string) => {
        setError(null);
        try {
            await teamsApi.join(teamId);
            const res = await teamsApi.list(stableFilters);
            setData(res);
        } catch (e) {
            setError(e instanceof Error ? e.message : "Failed to join team");
        }
    };

    return { data, loading, error, filters, setFilters, joinTeam };
}