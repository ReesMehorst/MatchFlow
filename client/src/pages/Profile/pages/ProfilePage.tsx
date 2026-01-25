import { useProfile } from "../hooks/useProfile";
import { AccountForm } from "../components/AccountForm";
import { JoinedTeams } from "../components/JoinedTeams";
import { DangerZone } from "../components/DangerZone";

export default function ProfilePage() {
    const { user, setUser, loading } = useProfile();

    if (loading) return <div className="container">Laden…</div>;
    if (!user) return null;

    return (
        <div className="container" style={{ padding: "32px 0", display: "grid", gap: 24 }}>
            <AccountForm
                user={user}
                onUpdated={(u) => setUser(prev => ({ ...prev!, ...u }))}
            />

            <JoinedTeams />

            <section className="card" style={{ padding: 24, opacity: 0.6 }}>
                <h2>Je posts</h2>
                <p>Komt later.</p>
            </section>

            <section className="card" style={{ padding: 24, opacity: 0.6 }}>
                <h2>Je comments</h2>
                <p>Komt later.</p>
            </section>

            <DangerZone />
        </div>
    );
}