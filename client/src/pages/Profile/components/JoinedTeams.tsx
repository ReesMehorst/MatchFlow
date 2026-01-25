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
            <h2>Je teams</h2>

            {teams.length === 0 ? (
                <p>Je bent nog geen lid van een team.</p>
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