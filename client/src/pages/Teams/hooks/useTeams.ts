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
                // fetch full list from compatibility endpoint
                const res = await teamsApi.list();
                // Apply client-side filters
                let items = res.items ?? [];

                if (stableFilters.search) {
                    const s = stableFilters.search.toLowerCase();
                    items = items.filter((i) => i.name.toLowerCase().includes(s));
                }

                if (stableFilters.tag) {
                    const t = stableFilters.tag.toUpperCase();
                    items = items.filter((i) => (i.tag ?? '').toUpperCase() === t);
                }

                if (stableFilters.minMembers !== "") {
                    const min = Number(stableFilters.minMembers ?? 0);
                    items = items.filter((i) => i.memberCount >= min);
                }

                if (stableFilters.maxMembers !== "") {
                    const max = Number(stableFilters.maxMembers ?? 0);
                    items = items.filter((i) => i.memberCount <= max);
                }

                // Sort
                items = (stableFilters.sort ?? 'members_desc') === 'members_desc'
                    ? items.sort((a, b) => b.memberCount - a.memberCount || a.name.localeCompare(b.name))
                    : (stableFilters.sort ?? 'members_desc') === 'members_asc'
                        ? items.sort((a, b) => a.memberCount - b.memberCount || a.name.localeCompare(b.name))
                        : (stableFilters.sort ?? 'members_desc') === 'name_asc'
                            ? items.sort((a, b) => a.name.localeCompare(b.name))
                            : items;

                const total = items.length;
                const page = stableFilters.page ?? 1;
                const pageSize = stableFilters.pageSize ?? 20;
                const paged = items.slice((page - 1) * pageSize, (page - 1) * pageSize + pageSize);

                const out = { items: paged, page, pageSize, total } as TeamListResponse;

                if (!cancelled) setData(out);
            } catch (e) {
                if (!cancelled) setError(e instanceof Error ? e.message : "Failed to load teams");
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
            const res = await teamsApi.list();
            setData(res);
        } catch (e) {
            setError(e instanceof Error ? e.message : "Failed to join team");
        }
    };

    return { data, loading, error, filters, setFilters, joinTeam };
}