import { useEffect, useState } from "react";
import type { Team } from "../constants/profileConstants";
import { getJoinedTeams } from "../constants/profileConstants";

export function JoinedTeams() {
    const [teams, setTeams] = useState<Team[]>([]);

    useEffect(() => {
        getJoinedTeams().then(setTeams);
    }, []);

    return (
        <section className="card" style={{ padding: 24 }}>
            <h2>Teams</h2>

            {teams.length === 0 ? (
                <p>Your not yet part of a team.</p>
            ) : (
                <ul style={{ paddingLeft: 18 }}>
                    {teams.map((t) => (
                        <li key={t.id}>{t.name}</li>
                    ))}
                </ul>
            )}
        </section>
    );
}