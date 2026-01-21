import { useMemo, useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { api } from "../../../services/api";
import "./CreateTeamPage.css";

type TeamDto = {
  id: string;
  name: string;
  tag: string;
  bio?: string | null;
  logoUrl?: string | null;
  ownerUserId: string;
  createdAt: string;
};

function normalizeTag(input: string) {
  return input.slice(0, 5);
}

export default function CreateTeamPage() {
  const nav = useNavigate();

  const [name, setName] = useState("");
  const [tag, setTag] = useState("");
  const [bio, setBio] = useState("");
  const [logoFile, setLogoFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!logoFile) {
      setPreviewUrl(null);
      return;
    }
    const url = URL.createObjectURL(logoFile);
    setPreviewUrl(url);
    return () => URL.revokeObjectURL(url);
  }, [logoFile]);

  const tagError = useMemo(() => {
    const t = tag.trim();
    if (t.length === 0) return null;
    if (t.length < 2) return "Tag must be at least 2 characters.";
    if (t.length > 5) return "Tag must be at most 5 characters.";
    return null;
  }, [tag]);

  const canSubmit = useMemo(() => {
    return (
      name.trim().length >= 2 &&
      tag.trim().length >= 2 &&
      tag.trim().length <= 5 &&
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
    // simple client-side validation
    if (!/^image\/(png|jpeg|jpg)$/.test(f.type)) {
      setError("Logo must be a PNG or JPEG image.");
      setLogoFile(null);
      return;
    }
    if (f.size > 2 * 1024 * 1024) {
      setError("Logo must be smaller than 2MB.");
      setLogoFile(null);
      return;
    }
    setLogoFile(f);
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    const cleanName = name.trim();
    const cleanTag = normalizeTag(tag).trim();

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
      // Build FormData - backend expects multipart/form-data
      const form = new FormData();
      form.append("Name", cleanName);
      form.append("Tag", cleanTag);
      form.append("Bio", bio.trim() ? bio.trim() : "");
      // ownerUserId omitted unless required by backend
      if (logoFile) form.append("LogoFile", logoFile);

      // Post multipart form-data to /api/teams
      const created = await api.post<TeamDto>("/teams", form, {
        headers: { /* let axios/set fetch set multipart boundary */ },
      });

      // If your api wrapper returns { data: TeamDto } adjust accordingly.
      let id: string | undefined;
      if ("id" in created && typeof created.id === "string") {
        id = created.id;
      } else if (
        created &&
        typeof created === "object" &&
        "data" in created &&
        created.data &&
        typeof (created as { data: unknown }).data === "object" &&
        "id" in (created as { data: { id?: unknown } }).data &&
        typeof ((created as { data: { id?: unknown } }).data as { id?: unknown }).id === "string"
      ) {
        id = ((created as { data: { id: string } }).data).id;
      }
      nav(`/teams/${id}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create team.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container createTeamPage">
      <div className="createTeamHeader">
        <div>
          <h1 className="createTeamTitle">Create team</h1>
          <p className="createTeamSubtitle">
            Pick a name and a short tag (2–5 characters). You can update details later.
          </p>
        </div>

        <Link className="btn btnGhost" to="/teams">
          Back to teams
        </Link>
      </div>

      <div className="card createTeamCard">
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
              onChange={(e) => setTag(normalizeTag(e.target.value))}
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
              <div style={{ marginTop: 8 }}>
                <img src={previewUrl} alt="logo preview" style={{ maxWidth: 160, maxHeight: 160 }} />
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