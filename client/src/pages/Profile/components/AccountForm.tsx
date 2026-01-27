import { useState } from "react";
import type { UserProfile } from "../constants/profileConstants";
import { updateProfile } from "../constants/profileConstants";

type Props = {
    user: UserProfile;
    onUpdated: (u: Partial<UserProfile>) => void;
};

export function AccountForm({ user, onUpdated }: Props) {
    const [displayName, setDisplayName] = useState(user.displayName);
    const [email, setEmail] = useState(user.email);
    const [password, setPassword] = useState("");

    const [success, setSuccess] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);

    async function submit(e: React.FormEvent) {
        e.preventDefault();
        setSuccess(false);
        setError(null);
        setSaving(true);

        try {
            await updateProfile({
                displayName,
                email,
                password: password || undefined
            });

            onUpdated({ displayName, email });
            setPassword("");
            setSuccess(true);
        } catch (err) {
            setError("Saving failed. Try again.");
        } finally {
            setSaving(false);
        }
    }

    return (
        <section className="card" style={{ padding: 24 }}>
            <h2>Account settings</h2>

            {success && (
                <div className="alert success">
                    Data succesfully changed.
                </div>
            )}

            {error && (
                <div className="alert error">
                    {error}
                </div>
            )}

            <form onSubmit={submit} style={{ display: "grid", gap: 16, maxWidth: 420 }}>
                <label>
                    Display name
                    <input
                        className="input"
                        value={displayName}
                        onChange={e => setDisplayName(e.target.value)}
                    />
                </label>

                <label>
                    Email
                    <input
                        className="input"
                        type="email"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                    />
                </label>

                <label>
                    Password
                    <input
                        className="input"
                        type="password"
                        placeholder="Leave empty if you don't want to change your password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                    />
                </label>

                <button className="btn btnPrimary" disabled={saving}>
                    {saving ? "Saving..." : "Save"}
                </button>
            </form>
        </section>
    );
}