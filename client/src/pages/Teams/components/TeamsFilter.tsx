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
    return (
        <section className="teamsFilters card" aria-label="Team filters">
            <div className="filterRow">
                <label className="filterField">
                    Name / Search
                    <input
                        className="input"
                        value={filters.search ?? ""}
                        onChange={(e) => setFilters({ search: e.target.value, page: 1 })}
                        placeholder="Search by team name..."
                    />
                </label>

                <label className="filterField">
                    Team tag (2–5)
                    <input
                        className="input"
                        value={filters.tag ?? ""}
                        onChange={(e) => setFilters({ tag: sanitizeTag(e.target.value), page: 1 })}
                        placeholder="ABC"
                        maxLength={5}
                    />
                </label>

                <label className="filterField">
                    Sort
                    <select
                        className="input"
                        value={filters.sort ?? "members_desc"}
                        onChange={(e) => setFilters({ sort: e.target.value as Sort, page: 1 })}
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
                        value={filters.minMembers ?? ""}
                        onChange={(e) =>
                            setFilters({
                                minMembers: e.target.value === "" ? "" : Number(e.target.value),
                                page: 1,
                            })
                        }
                        placeholder="0"
                    />
                </label>

                <label className="filterField">
                    Max members
                    <input
                        className="input"
                        type="number"
                        min={0}
                        value={filters.maxMembers ?? ""}
                        onChange={(e) =>
                            setFilters({
                                maxMembers: e.target.value === "" ? "" : Number(e.target.value),
                                page: 1,
                            })
                        }
                        placeholder="999"
                    />
                </label>

                <div className="filterActions">
                    <button className="btn btnSecondary" type="button" onClick={onReset}>
                        Reset
                    </button>
                </div>
            </div>
        </section>
    );
}