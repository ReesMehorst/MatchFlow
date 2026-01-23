import { useCallback } from "react";
import { Link } from "react-router-dom";
import { useTeams } from "../hooks/useTeams";
import TeamCard from "../components/TeamCard";
import TeamsFilters from "../components/TeamsFilter";
import TeamsPager from "../components/TeamsPager";
import "./teams.css";

export default function TeamsPage() {
    const { data, loading, error, filters, setFilters, joinTeam } = useTeams();

    const items = data?.items ?? [];
    const total = data?.total ?? 0;
    const page = data?.page ?? 1;
    const pageSize = data?.pageSize ?? 20;

    const canPrev = page > 1;
    const canNext = page * pageSize < total;

    const onJoin = useCallback(async (teamId: string) => {
        await joinTeam(teamId);
    }, [joinTeam]);

    const onReset = useCallback(() => {
        setFilters({
            search: "",
            tag: "",
            minMembers: "",
            maxMembers: "",
            sort: "members_desc",
            page: 1,
        });
    }, [setFilters]);

    return (
        <div className="container teamsPage page">
            <div className="pageHeader">
                <div>
                    <h1 className="pageTitle">Teams</h1>
                    <p className="pageSubtitle">Find teams and join them.</p>
                </div>

                <Link className="btn btnPrimary" to="/teams/create">
                    Create team
                </Link>
            </div>

            <TeamsFilters filters={filters} setFilters={setFilters} onReset={onReset} />

            {error && (
                <div className="pageMessage" role="alert" aria-live="polite">
                    {error}
                </div>
            )}

            {loading && <div className="pageMessage">Loading teams...</div>}

            {!loading && !error && items.length === 0 && (
                <div className="pageMessage">No teams found. Try different filters.</div>
            )}

            <section className="teamsGrid" aria-label="Teams list">
                {items.map((t) => (
                    <TeamCard key={t.id} team={t} onJoin={onJoin} />
                ))}
            </section>

            <TeamsPager
                page={page}
                total={total}
                pageSize={pageSize}
                canPrev={canPrev}
                canNext={canNext}
                onPrev={() => setFilters({ page: (filters.page ?? 1) - 1 })}
                onNext={() => setFilters({ page: (filters.page ?? 1) + 1 })}
            />
        </div>
    );
}