import { Link } from "react-router-dom";
import { useTeams } from "../hooks/useTeams";
import type { TeamSummary } from "../constants/teamsConstants";
import "./TeamsPage.css";

export default function TeamsPage() {
    const { data, loading, error, filters, setFilters, joinTeam } = useTeams();

    const items = data?.items ?? [];
    const total = data?.total ?? 0;
    const page = data?.page ?? 1;
    const pageSize = data?.pageSize ?? 20;

    const canPrev = page > 1;
    const canNext = page * pageSize < total;

    const onJoin = async (team: TeamSummary) => {
        await joinTeam(team.id);
    };

    return (
        <div className="container teamsPage">
            <div className="teamsHeader">
                <div>
                    <h1 className="teamsTitle">Teams</h1>
                    <p className="teamsSubtitle">Find teams and join them.</p>
                </div>

                <Link className="btn btnPrimary" to="/teams/create">
                    Create team
                </Link>
            </div>

            <section className="teamsFilters card">
                <div className="filterRow">
                    <label className="filterField">
                        Name / Search
                        <input
                            className="input"
                            value={filters.search ?? ""}
                            onChange={(e) => setFilters({ search: e.target.value })}
                            placeholder="Search by team name..."
                        />
                    </label>

                    <label className="filterField">
                        Team tag (2–5)
                        <input
                            className="input"
                            value={filters.tag ?? ""}
                            onChange={(e) => {
                                const v = e.target.value.toUpperCase().replace(/[^A-Z0-9]/g, "");
                                setFilters({ tag: v });
                            }}
                            placeholder="ABC"
                            maxLength={5}
                        />
                    </label>

                    <label className="filterField">
                        Sort
                        <select
                            className="input"
                            value={filters.sort ?? "members_desc"}
                            onChange={(e) =>
                                setFilters({
                                    sort: e.target.value as "created_desc" | "name_asc" | "members_desc" | "members_asc",
                                })
                            }
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
                                setFilters({ minMembers: e.target.value === "" ? "" : Number(e.target.value) })
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
                                setFilters({ maxMembers: e.target.value === "" ? "" : Number(e.target.value) })
                            }
                            placeholder="999"
                        />
                    </label>

                    <div className="filterActions">
                        <button
                            className="btn btnSecondary"
                            type="button"
                            onClick={() =>
                                setFilters({
                                    search: "",
                                    tag: "",
                                    minMembers: "",
                                    maxMembers: "",
                                    sort: "members_desc",
                                    page: 1,
                                })
                            }
                        >
                            Reset
                        </button>
                    </div>
                </div>
            </section>

            {error && (
                <div className="teamsMessage" role="alert" aria-live="polite">
                    {error}
                </div>
            )}

            {loading && <div className="teamsMessage">Loading teams...</div>}

            {!loading && !error && items.length === 0 && (
                <div className="teamsMessage">No teams found. Try different filters.</div>
            )}

            <section className="teamsGrid">
                {items.map((t) => (
                    <article key={t.id} className="teamCard card">
                        <div className="teamTop">
                            <div className="teamNameRow">
                                <h2 className="teamName">{t.name}</h2>
                                <span className="teamTag">{t.tag}</span>
                            </div>

                            <div className="teamMeta">
                                <span>{t.memberCount} members</span>
                            </div>
                        </div>

                        {t.bio && <p className="teamDesc">{t.bio}</p>}

                        <div className="teamActions">
                            <Link className="btn btnGhost" to={`/teams/${t.id}`}>View</Link>

                            <button
                                className="btn btnPrimary"
                                type="button"
                                onClick={() => onJoin(t)}
                                disabled={t.isMember}
                            >
                                {t.isMember ? "Joined" : "Join"}
                            </button>
                        </div>
                    </article>
                ))}
            </section>

            <div className="teamsPager">
                <button
                    className="btn btnSecondary"
                    type="button"
                    disabled={!canPrev}
                    onClick={() => setFilters({ page: (filters.page ?? 1) - 1 })}
                >
                    Prev
                </button>

                <span className="pagerText">
                    Page {page} • {total} total
                </span>

                <button
                    className="btn btnSecondary"
                    type="button"
                    disabled={!canNext}
                    onClick={() => setFilters({ page: (filters.page ?? 1) + 1 })}
                >
                    Next
                </button>
            </div>
        </div>
    );
}