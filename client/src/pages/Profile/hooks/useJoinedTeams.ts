import { useEffect, useState } from "react";
import { getMyTeams } from "../constants/profileConstants";
import type { Team } from "../constants/profileConstants";

export function useJoinedTeams() {
    const [teams, setTeams] = useState<Team[]>([]);

    useEffect(() => {
        getMyTeams().then(setTeams);
    }, []);

    return teams;
}