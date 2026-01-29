import { Link } from "react-router-dom";
import type { TeamSummary } from "../constants/teamsConstants";

type Props = {
    team: TeamSummary;
    onJoin?: (teamId: string) => Promise<void> | void;
    readonly?: boolean;
};

export default function TeamCard({ team, onJoin, readonly }: Props) {
    return (
        <article className="teamCard card">
            <div className="teamTop">
                <div className="teamNameRow">
                    <h2 className="teamName">{team.name}</h2>
                    <span className="teamTag">{team.tag}</span>
                </div>

                <div className="teamMeta">
                    <span>{team.memberCount} members</span>
                </div>
            </div>

            {team.bio && <p className="teamDesc">{team.bio}</p>}

            <div className="teamActions">
                <Link className="btn btnGhost" to={`/teams/${team.id}`}>
                    View
                </Link>

                {!readonly && onJoin && (
                    <button
                        className="btn btnPrimary"
                        type="button"
                        onClick={() => onJoin(team.id)}
                        disabled={team.isMember}
                    >
                        {team.isMember ? "Joined" : "Join"}
                    </button>
                )}
            </div>
        </article>
    );
}