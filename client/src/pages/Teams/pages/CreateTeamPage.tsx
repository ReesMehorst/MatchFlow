import React from "react";
import { api } from "../../../services/api";
import { useAuth } from "../../../context/useAuth";

export default function CreateTeamPage() {
    const [name, setName] = React.useState("");
    const [tag, setTag] = React.useState("");
    const [bio, setBio] = React.useState("");
    const [ownerId, setOwnerId] = React.useState<string | null>(null);
    const [logo, setLogo] = React.useState<File | null>(null);
    const [message, setMessage] = React.useState<string | null>(null);

    const auth = useAuth();

    async function loadMe() {
        if (auth?.user?.id) {
            setOwnerId(auth.user.id);
            return;
        }
        try {
            const data = await api.get<{ id: string }>("/auth/me");
            if (data?.id) setOwnerId(data.id);
            else setMessage("Authenticated but no user id returned");
        } catch {
            setMessage("Error fetching current user");
        }
    }

    React.useEffect(() => {
        loadMe();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [auth?.user?.id]);

    function onFileChange(e: React.ChangeEvent<HTMLInputElement>) {
        const f = e.target.files?.[0] ?? null;
        setLogo(f);
    }

    async function onSubmit(e: React.FormEvent) {
        e.preventDefault();

        const fd = new FormData();
        fd.append("Name", name);
        fd.append("Tag", tag);
        if (bio) fd.append("Bio", bio);
        if (ownerId) fd.append("OwnerUserId", ownerId);
        if (logo) fd.append("LogoFile", logo, logo.name);

        try {
            await api.post("/team", fd as unknown as FormData);
            setMessage("Team created successfully");
        } catch (err) {
            setMessage(`Network or unexpected error: ${String(err)}`);
        }
    }

    return (
        <div className="container page">
            <div className="pageHeader">
                <div>
                    <h1 className="pageTitle">Create team</h1>
                    <p className="pageSubtitle">
                        Set up a new team with a name, tag, bio and logo.
                    </p>
                </div>
            </div>

            <div className="pageCard card">
                <form className="formGrid" onSubmit={onSubmit}>
                    <div className="formField">
                        <label htmlFor="name">Team name</label>
                        <input
                            id="name"
                            className="input"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            required
                        />
                    </div>

                    <div className="formField">
                        <label htmlFor="tag">Team tag (2–5)</label>
                        <input
                            id="tag"
                            className="input"
                            value={tag}
                            onChange={(e) => setTag(e.target.value)}
                            maxLength={5}
                            required
                        />
                    </div>

                    <div className="formField formFieldFull">
                        <label htmlFor="bio">Bio</label>
                        <textarea
                            id="bio"
                            className="input textarea"
                            value={bio}
                            onChange={(e) => setBio(e.target.value)}
                        />
                    </div>

                    <div className="formField formFieldFull">
                        <label htmlFor="logo">Team logo</label>
                        <input
                            id="logo"
                            className="input"
                            type="file"
                            accept="image/png,image/jpeg"
                            onChange={onFileChange}
                        />
                        {logo && (
                            <div className="logoPreview">
                                <img src={URL.createObjectURL(logo)} alt="Logo preview" />
                            </div>
                        )}
                    </div>

                    <div className="formActions formFieldFull">
                        <button type="submit" className="btn btnPrimary">
                            Create team
                        </button>
                        <button
                            type="reset"
                            className="btn btnSecondary"
                            onClick={() => {
                                setName("");
                                setTag("");
                                setBio("");
                                setLogo(null);
                                setMessage(null);
                            }}
                        >
                            Reset
                        </button>
                    </div>
                </form>

                {message && (
                    <div className="pageMessage" role="status">
                        {message}
                    </div>
                )}
            </div>
        </div>
    );
}