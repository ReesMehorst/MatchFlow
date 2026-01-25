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

    async function submit(e: React.FormEvent) {
        e.preventDefault();

        await updateProfile({
            displayName,
            email,
            password: password || undefined
        });

        onUpdated({ displayName, email });
        setPassword("");
    }

    return (
        <section className="card" style={{ padding: 24 }}>
            <h2>Account settings</h2>

            <form onSubmit={submit} style={{ display: "grid", gap: 16, maxWidth: 420 }}>
                <label>
                    Display name
                    <input className="input" value={displayName} onChange={e => setDisplayName(e.target.value)} />
                </label>

                <label>
                    Email
                    <input className="input" type="email" value={email} onChange={e => setEmail(e.target.value)} />
                </label>

                <label>
                    Password
                    <input
                        className="input"
                        type="password"
                        placeholder="Laat leeg om niet te wijzigen"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                    />
                </label>

                <button className="btn btnPrimary">Save</button>
            </form>
        </section>
    );
}