import { useState } from "react";
import { deleteProfile } from "../constants/profileConstants";

export function DangerZone() {
    const [confirm, setConfirm] = useState(false);

    async function handleDelete() {
        await deleteProfile();
        localStorage.removeItem("mf_token");
        window.location.href = "/login";
    }

    return (
        <section className="card" style={{ padding: 24 }}>
            <h2>Account verwijderen</h2>

            {!confirm ? (
                <button className="btn btnGhost" onClick={() => setConfirm(true)}>
                    Account verwijderen
                </button>
            ) : (
                <div style={{ display: "grid", gap: 12 }}>
                    <p>Dit kan niet ongedaan worden gemaakt.</p>
                    <div style={{ display: "flex", gap: 12 }}>
                        <button className="btn btnSecondary" onClick={() => setConfirm(false)}>
                            Annuleren
                        </button>
                        <button className="btn btnPrimary" onClick={handleDelete}>
                            Definitief verwijderen
                        </button>
                    </div>
                </div>
            )}
        </section>
    );
}