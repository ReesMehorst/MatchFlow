import { useState } from "react";
import { deleteProfile } from "../constants/profileConstants";

export function DangerZone() {
    const [confirm, setConfirm] = useState(false);
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    async function handleDelete() {
        if (!password) {
            setError("Password is required.");
            return;
        }

        try {
            setLoading(true);
            setError(null);

            await deleteProfile(password);

            localStorage.removeItem("mf_token");
            window.location.href = "/login";
        } catch (err) {
            setError("Password incorrect or deletion failed.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <section className="card" style={{ padding: 24 }}>
            <h2>Delete account</h2>

            {!confirm ? (
                <button className="btn btnGhost" onClick={() => setConfirm(true)}>
                    Delete account
                </button>
            ) : (
                <div style={{ display: "grid", gap: 12 }}>
                    <p>This action can't be undone.</p>

                    <input
                        className="input"
                        type="password"
                        placeholder="Enter your password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />

                    {error && <p style={{ color: "red" }}>{error}</p>}

                    <div style={{ display: "flex", gap: 12 }}>
                        <button
                            className="btn btnSecondary"
                            onClick={() => {
                                setConfirm(false);
                                setPassword("");
                                setError(null);
                            }}
                        >
                            Cancel
                        </button>

                        <button
                            className="btn btnPrimary"
                            onClick={handleDelete}
                            disabled={loading}
                        >
                            {loading ? "Removing..." : "Remove permanently"}
                        </button>
                    </div>
                </div>
            )}
        </section>
    );
}