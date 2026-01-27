import { useEffect, useState } from "react";
import type { Team } from "../constants/profileConstants";
import { getMyTeams } from "../constants/profileConstants";

export function JoinedTeams() {
    const [teams, setTeams] = useState<Team[]>([]);

    useEffect(() => {
        getMyTeams()
            .then(setTeams)
            .catch(err => {
                console.error("Failed to fetch teams:", err);
                setTeams([]);
            });
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