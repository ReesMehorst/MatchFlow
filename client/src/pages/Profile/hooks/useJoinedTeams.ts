import { useEffect, useState } from "react";
import { getJoinedTeams } from "../constants/profileConstants";
import type { Team } from "../constants/profileConstants";

export function useJoinedTeams() {
    const [teams, setTeams] = useState<Team[]>([]);

    useEffect(() => {
        getJoinedTeams().then(setTeams);
    }, []);

    return teams;
}