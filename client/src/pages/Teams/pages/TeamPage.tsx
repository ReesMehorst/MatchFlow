import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { teamsApi, type TeamDto, type TeamMemberDto, type TeamMatch } from "../constants/teamsConstants";
import "./teams.css";

export default function TeamPage() {
  const { id } = useParams<{ id: string }>();
  const [team, setTeam] = useState<TeamDto | null>(null);
  const [members, setMembers] = useState<TeamMemberDto[]>([]);
  const [matches, setMatches] = useState<TeamMatch[]>([]); // error want nog niet gebruikt
  const [tab, setTab] = useState<"overview" | "members" | "matches">("overview");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    setError(null);

    (async () => {
      try {
        const t = await teamsApi.get(id);
        setTeam(t);
        // Fetcht alle members en filtert op teamId (endpoint maken bij /TeamMember?)
        const allMembers = await teamsApi.getMembers();
        setMembers(allMembers.filter((m) => m.teamId === id));
        const m = await teamsApi.getMatches(id);
        setMatches(m);
      } catch (e) {
        setError(e instanceof Error ? e.message : "Failed to load team");
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  if (!id) return <div className="pageMessage">No team specified.</div>;

  return (
    <div className="container teamPage page">
      {loading && <div className="pageMessage">Loading...</div>}
      {error && <div className="pageMessage" role="alert">{error}</div>}

      {team && (
        <>
          <header className="teamHeader">
            <img className="teamLogo" src={team.logoUrl ?? "/images/team-default.png"} alt={`${team.name} logo`} />
            <div>
              <h1>{team.name}</h1>
              <div className="muted">{team.tag}</div>
              <p>{team.bio}</p>
            </div>
          </header>

          <nav className="tabs">
            <button className="btn" onClick={() => setTab("overview")} aria-pressed={tab === "overview"}>Overview</button>
            <button className="btn" onClick={() => setTab("members")} aria-pressed={tab === "members"}>Members ({members.length})</button>
            <button className="btn" onClick={() => setTab("matches")} aria-pressed={tab === "matches"}>Matches</button>
          </nav>

          <main>
            {tab === "overview" && (
              <section aria-label="Team overview">
                <h2>Overview</h2>
                <dl>
                  <dt>Owner</dt><dd>{team.ownerUserId}</dd>
                  <dt>Created</dt><dd>{new Date(team.createdAt).toLocaleString()}</dd>
                </dl>
                {/* add meer logica voor Overview, posts, win loss, highlights. */}
              </section>
            )}

            {tab === "members" && (
              <section aria-label="Team members">
                <h2>Members</h2>
                <table className="membersTable">
                  <thead><tr><th>Role</th><th>User</th><th>Joined</th><th>Left</th></tr></thead>
                  <tbody>
                    {members.map((m) => (
                      <tr key={m.id}>
                        <td>{m.role}</td>
                            <td>{m.userId}</td> {/* Username fetchen logica ipv id */ }
                        <td>{new Date(m.joinedAt).toLocaleDateString()}</td>
                        <td>{m.leftAt ? new Date(m.leftAt).toLocaleDateString() : ""}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </section>
            )}

            {tab === "matches" && (
              <section aria-label="Team matches">
                <h2>Matches</h2>
                    {/*Matches logica hier*/}
              </section>
            )}
          </main>
        </>
      )}
    </div>
  );
}