import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/useAuth";

export default function RegisterPage() {
    const nav = useNavigate();
    const { register } = useAuth();

    const [displayName, setDisplayName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (password !== confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        setLoading(true);
        try {
            await register({ email, password, displayName });
            nav("/");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Registration failed");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container" style={{ padding: "40px 0" }}>
            <div className="card" style={{ padding: 18, maxWidth: 460, margin: "0 auto" }}>
                <h1 style={{ marginTop: 0 }}>Create account</h1>

                {error && (
                    <p role="alert" aria-live="polite" style={{ marginTop: 10 }}>
                        {error}
                    </p>
                )}

                <form onSubmit={onSubmit}>
                    <label style={{ display: "block" }}>
                        Display name
                        <input
                            className="input"
                            value={displayName}
                            onChange={(e) => setDisplayName(e.target.value)}
                            type="text"
                            autoComplete="nickname"
                            required
                        />
                    </label>

                    <label style={{ display: "block", marginTop: 12 }}>
                        Email
                        <input
                            className="input"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            type="email"
                            autoComplete="email"
                            required
                        />
                    </label>

                    <label style={{ display: "block", marginTop: 12 }}>
                        Password
                        <input
                            className="input"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            type="password"
                            autoComplete="new-password"
                            required
                            minLength={8}
                        />
                    </label>

                    <label style={{ display: "block", marginTop: 12 }}>
                        Confirm password
                        <input
                            className="input"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            type="password"
                            autoComplete="new-password"
                            required
                            minLength={8}
                        />
                    </label>

                    <button
                        className="btn btnPrimary"
                        disabled={loading}
                        style={{ marginTop: 14, width: "100%" }}
                    >
                        {loading ? "Creating account..." : "Create account"}
                    </button>
                </form>

                <p style={{ marginBottom: 0, marginTop: 12, color: "var(--muted)" }}>
                    Already have an account? <Link to="/login">Log in</Link>
                </p>
            </div>
        </div>
    );
}