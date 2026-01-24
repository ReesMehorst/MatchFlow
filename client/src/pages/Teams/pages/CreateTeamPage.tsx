import React, { useState, ChangeEvent, FormEvent } from 'react';

// Simple CreateTeam page example that:
// 1) fetches /api/auth/me to show current user
// 2) posts form data (multipart/form-data) to /api/team to create a team
// Uses console.log liberally so you can follow the steps in the browser devtools

export default function CreateTeamPage() {
    const [name, setName] = useState('');
    const [tag, setTag] = useState('');
    const [bio, setBio] = useState('');
    const [ownerId, setOwnerId] = useState<string | null>(null);
    const [logo, setLogo] = useState<File | null>(null);
    const [message, setMessage] = useState<string | null>(null);

    // Attempt to read token from localStorage (adjust to your auth flow)
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;

    async function loadMe() {
        console.log('[CreateTeamPage] loadMe: starting');
        if (!token) {
            console.log('[CreateTeamPage] no token found in localStorage');
            setMessage('Not authenticated');
            return;
        }

        try {
            const res = await fetch('/api/auth/me', {
                headers: { Authorization: `Bearer ${token}` },
            });
            console.log('[CreateTeamPage] /api/auth/me response', res);
            if (!res.ok) {
                const text = await res.text();
                console.log('[CreateTeamPage] /api/auth/me error body', text);
                setMessage('Failed to get current user');
                return;
            }
            const data = await res.json();
            console.log('[CreateTeamPage] /api/auth/me data', data);
            setOwnerId(data.id as string);
        } catch (err) {
            console.error('[CreateTeamPage] loadMe error', err);
            setMessage('Error fetching current user');
        }
    }

    // Run once on mount
    React.useEffect(() => {
        loadMe();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    function onFileChange(e: ChangeEvent<HTMLInputElement>) {
        const f = e.target.files && e.target.files[0];
        setLogo(f ?? null);
        console.log('[CreateTeamPage] selected file', f);
    }

    async function onSubmit(e: FormEvent) {
        e.preventDefault();
        console.log('[CreateTeamPage] onSubmit: building form data');

        const fd = new FormData();
        fd.append('Name', name);
        fd.append('Tag', tag);
        if (bio) fd.append('Bio', bio);
        // You can either attach OwnerUserId explicitly, or rely on server-side claim resolution
        if (ownerId) {
            console.log('[CreateTeamPage] attaching ownerId to form data', ownerId);
            fd.append('OwnerUserId', ownerId);
        }
        if (logo) fd.append('LogoFile', logo, logo.name);

        try {
            console.log('[CreateTeamPage] sending POST /api/team');
            const res = await fetch('/api/team', {
                method: 'POST',
                headers: token ? { Authorization: `Bearer ${token}` } : undefined,
                body: fd,
            });
            console.log('[CreateTeamPage] response', res);
            const text = await res.text();
            console.log('[CreateTeamPage] response body', text);
            if (!res.ok) {
                setMessage(`Failed: ${res.status} - ${text}`);
                return;
            }

            // try parse JSON
            try {
                const json = JSON.parse(text);
                console.log('[CreateTeamPage] created team', json);
                setMessage('Team created successfully');
            } catch (parseErr) {
                console.log('[CreateTeamPage] created team (non-json)', text);
                setMessage('Team created (response not JSON)');
            }
        } catch (err) {
            console.error('[CreateTeamPage] submit error', err);
            setMessage('Network or unexpected error');
        }
    }

    return (
        <div style={{ maxWidth: 640, margin: '0 auto' }}>
            <h2>Create Team</h2>
            <p>Current owner id: {ownerId ?? 'unknown'}</p>
            <form onSubmit={onSubmit}>
                <div>
                    <label>Name</label>
                    <input value={name} onChange={e => setName(e.target.value)} required />
                </div>
                <div>
                    <label>Tag</label>
                    <input value={tag} onChange={e => setTag(e.target.value)} required />
                </div>
                <div>
                    <label>Bio</label>
                    <textarea value={bio} onChange={e => setBio(e.target.value)} />
                </div>
                <div>
                    <label>Logo</label>
                    <input type="file" accept="image/png,image/jpeg" onChange={onFileChange} />
                </div>
                <div>
                    <button type="submit">Create</button>
                </div>
            </form>
            {message && <div style={{ marginTop: 12 }}>{message}</div>}
        </div>
    );
}
