import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../../context/useAuth";

export default function LoginPage() {
    const nav = useNavigate();
    const { login } = useAuth();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);
        try {
            await login({ email, password });
            nav("/");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Login failed");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container" style={{ padding: "40px 0" }}>
            <div className="card" style={{ padding: 18, maxWidth: 460, margin: "0 auto" }}>
                <h1 style={{ marginTop: 0 }}>Log in</h1>

                {error && <p style={{ color: "var(--danger)" }}>{error}</p>}

                <form onSubmit={onSubmit}>
                    <label>
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
                            autoComplete="current-password"
                            required
                        />
                    </label>

                    <button className="btn btnPrimary" disabled={loading} style={{ marginTop: 14, width: "100%" }}>
                        {loading ? "Signing in..." : "Log in"}
                    </button>
                </form>

                <p style={{ marginBottom: 0, marginTop: 12, color: "var(--muted)" }}>
                    No account? <Link to="/register">Register</Link>
                </p>
            </div>
        </div>
    );
}