import React from "react";
import type { TeamFilters as Filters } from "../constants/teamsConstants";

type Sort =
    | "created_desc"
    | "name_asc"
    | "members_desc"
    | "members_asc";

type Props = {
    filters: Filters;
    setFilters: (patch: Partial<Filters>) => void;
    onReset: () => void;
};

function sanitizeTag(raw: string) {
    return raw.toUpperCase().replace(/[^A-Z0-9]/g, "").slice(0, 5);
}

export default function TeamsFilters({ filters, setFilters, onReset }: Props) {
    const [local, setLocal] = React.useState<Filters>({
        search: filters.search ?? "",
        tag: filters.tag ?? "",
        minMembers: filters.minMembers ?? "",
        maxMembers: filters.maxMembers ?? "",
        sort: filters.sort ?? "members_desc",
        page: filters.page ?? 1,
        pageSize: filters.pageSize ?? 20,
    });

    React.useEffect(() => {
        setLocal((l) => ({ ...l, search: filters.search ?? "", tag: filters.tag ?? "", sort: filters.sort ?? "members_desc" }));
    }, [filters.search, filters.tag, filters.sort]);

    function apply() {
        setFilters({
            search: local.search,
            tag: local.tag,
            minMembers: local.minMembers,
            maxMembers: local.maxMembers,
            sort: local.sort,
            page: 1,
        });
    }

    return (
        <section className="teamsFilters card" aria-label="Team filters">
            <div className="filterRow">
                <label className="filterField">
                    Name / Search
                    <input
                        className="input"
                        value={local.search ?? ""}
                        onChange={(e) => setLocal({ ...local, search: e.target.value })}
                        placeholder="Search by team name..."
                    />
                </label>

                <label className="filterField">
                    Team tag (2–5)
                    <input
                        className="input"
                        value={local.tag ?? ""}
                        onChange={(e) => setLocal({ ...local, tag: sanitizeTag(e.target.value) })}
                        placeholder="ABC"
                        maxLength={5}
                    />
                </label>

                <label className="filterField">
                    Sort
                    <select
                        className="input"
                        value={local.sort ?? "members_desc"}
                        onChange={(e) => setLocal({ ...local, sort: e.target.value as Sort })}
                    >
                        <option value="members_desc">Members (high → low)</option>
                        <option value="members_asc">Members (low → high)</option>
                        <option value="name_asc">Name (A → Z)</option>
                        <option value="created_desc">Newest</option>
                    </select>
                </label>
            </div>

            <div className="filterRow">
                <label className="filterField">
                    Min members
                    <input
                        className="input"
                        type="number"
                        min={0}
                        value={local.minMembers ?? ""}
                        onChange={(e) => setLocal({ ...local, minMembers: e.target.value === "" ? "" : Number(e.target.value) })}
                        placeholder="0"
                    />
                </label>

                <label className="filterField">
                    Max members
                    <input
                        className="input"
                        type="number"
                        min={0}
                        value={local.maxMembers ?? ""}
                        onChange={(e) => setLocal({ ...local, maxMembers: e.target.value === "" ? "" : Number(e.target.value) })}
                        placeholder="999"
                    />
                </label>

                <div className="filterActions">
                    <button className="btn btnSecondary" type="button" onClick={apply}>
                        Search
                    </button>
                    <button className="btn btnSecondary" type="button" onClick={onReset}>
                        Reset
                    </button>
                </div>
            </div>
        </section>
    );
}
