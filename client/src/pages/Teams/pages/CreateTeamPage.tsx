import { useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { api } from "../../../services/api";
import { useImagePreview } from "../hooks/useImagePreview";
import {
    normalizeTeamTag,
    unwrapApiData,
    validateLogoFile,
    validateTeamTag,
} from "../utils/TeamFormUtils";
import "./teams.css";

type TeamDto = {
    id: string;
    name: string;
    tag: string;
    bio?: string | null;
    logoUrl?: string | null;
    ownerUserId: string;
    createdAt: string;
};

export default function CreateTeamPage() {
    const nav = useNavigate();

    const [name, setName] = useState("");
    const [tag, setTag] = useState("");
    const [bio, setBio] = useState("");
    const [logoFile, setLogoFile] = useState<File | null>(null);

    const previewUrl = useImagePreview(logoFile);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const tagError = useMemo(() => validateTeamTag(tag), [tag]);

    const canSubmit = useMemo(() => {
        const cleanName = name.trim();
        const cleanTag = normalizeTeamTag(tag).trim();
        return (
            cleanName.length >= 2 &&
            cleanTag.length >= 2 &&
            cleanTag.length <= 5 &&
            !loading
        );
    }, [name, tag, loading]);

    const onFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setError(null);
        const f = e.target.files && e.target.files[0];
        if (!f) {
            setLogoFile(null);
            return;
        }

        const fileError = validateLogoFile(f);
        if (fileError) {
            setError(fileError);
            setLogoFile(null);
            return;
        }

        setLogoFile(f);
    };

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        const cleanName = name.trim();
        const cleanTag = normalizeTeamTag(tag).trim();

        if (cleanName.length < 2) {
            setError("Team name must be at least 2 characters.");
            return;
        }
        if (cleanTag.length < 2 || cleanTag.length > 5) {
            setError("Team tag must be 2–5 characters.");
            return;
        }

        setLoading(true);
        try {
            const form = new FormData();
            form.append("Name", cleanName);
            form.append("Tag", cleanTag);
            form.append("Bio", bio.trim() ? bio.trim() : "");
            if (logoFile) form.append("LogoFile", logoFile);

            const raw = await api.post<TeamDto>("/teams", form, {
                headers: { /* boundary set automatically */ },
            });

            const created = unwrapApiData<TeamDto>(raw);
            if (!created?.id) {
                throw new Error("Team created but no id was returned by the API.");
            }

            nav(`/teams/${created.id}`);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to create team.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container createTeamPage page">
            <div className="pageHeader">
                <div>
                    <h1 className="pageTitle">Create team</h1>
                    <p className="pageSubtitle">
                        Pick a name and a short tag (2–5 characters). You can update details later.
                    </p>
                </div>

                <Link className="btn btnGhost" to="/teams">
                    Back to teams
                </Link>
            </div>

            <div className="card pageCard">
                {error && (
                    <div className="formError" role="alert" aria-live="polite">
                        {error}
                    </div>
                )}

                <form onSubmit={onSubmit} className="formGrid" encType="multipart/form-data">
                    <label className="formField">
                        Team name
                        <input
                            className="input"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            placeholder="e.g. MatchFlow United"
                            required
                            minLength={2}
                        />
                    </label>

                    <label className="formField">
                        Team tag (2–5)
                        <input
                            className="input"
                            value={tag}
                            onChange={(e) => setTag(normalizeTeamTag(e.target.value))}
                            placeholder="ABC"
                            maxLength={5}
                            required
                        />
                        {tagError && <span className="hintError">{tagError}</span>}
                        {!tagError && <span className="hint">Tag is <strong>case sensitive</strong>.</span>}
                    </label>

                    <label className="formField formFieldFull">
                        Bio (optional)
                        <textarea
                            className="input textarea"
                            value={bio}
                            onChange={(e) => setBio(e.target.value)}
                            placeholder="What is this team about?"
                            rows={4}
                        />
                    </label>

                    <label className="formField formFieldFull">
                        Logo (optional) — .png or .jpg
                        <input
                            className="input"
                            type="file"
                            accept=".png,.jpg,.jpeg,image/png,image/jpeg"
                            onChange={onFileChange}
                        />

                        {previewUrl && (
                            <div className="logoPreview">
                                <img src={previewUrl} alt="logo preview" />
                            </div>
                        )}

                        <div className="hint">Tip: keep logo small. Max 2MB recommended.</div>
                    </label>

                    <div className="formActions">
                        <button className="btn btnPrimary" type="submit" disabled={!canSubmit}>
                            {loading ? "Creating..." : "Create team"}
                        </button>

                        <Link className="btn btnSecondary" to="/teams">
                            Cancel
                        </Link>
                    </div>
                </form>
            </div>
        </div>
    );
}