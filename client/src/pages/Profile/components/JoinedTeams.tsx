import { useEffect, useState } from "react";
import TeamCard from "../../Teams/components/TeamCard";
import { getMyTeams } from "../constants/profileConstants";
import type { TeamSummary } from "../../Teams/constants/teamsConstants";
import "../../Teams/pages/teams.css";
export function JoinedTeams() {
    const [teams, setTeams] = useState<TeamSummary[]>([]);

    useEffect(() => {
        getMyTeams()
            .then(apiTeams => {
                const mapped: TeamSummary[] = apiTeams.map(t => ({
                    id: t.id,
                    name: t.name,
                    tag: t.tag,
                    bio: t.bio,
                    memberCount: 0,   // backend levert dit niet
                    isMember: true    // logisch: dit zijn joined teams
                }));

                setTeams(mapped);
            })
            .catch(err => {
                console.error("Failed to fetch teams:", err);
                setTeams([]);
            });
    }, []);

    return (
        <section className="card" style={{ padding: 24 }}>
            <h2>Teams</h2>

            {teams.length === 0 ? (
                <p>You are not yet part of a team.</p>
            ) : (
                <div className="teamsGrid">
                    {teams.map(team => (
                        <TeamCard
                            key={team.id}
                            team={team}
                            readonly
                        />
                    ))}
                </div>
            )}
        </section>
    );
}