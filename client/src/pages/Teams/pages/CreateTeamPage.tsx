import React from 'react';
import { api } from '../../../services/api';
import { useAuth } from '../../../context/useAuth';

// Simple CreateTeam page example that:
// 1) fetches /api/auth/me to show current user
// 2) posts form data (multipart/form-data) to /api/team to create a team
// Uses console.log liberally so you can follow the steps in the browser devtools

export default function CreateTeamPage() {
    const [name, setName] = React.useState('');
    const [tag, setTag] = React.useState('');
    const [bio, setBio] = React.useState('');
    const [ownerId, setOwnerId] = React.useState<string | null>(null);
    const [logo, setLogo] = React.useState<File | null>(null);
    const [message, setMessage] = React.useState<string | null>(null);

    // Token is stored by AuthProvider under "mf_token"; prefer context when available
    const auth = useAuth();
    const token = typeof window !== 'undefined' ? localStorage.getItem('mf_token') : null;

    async function loadMe() {
        console.log('[CreateTeamPage] loadMe: starting');
        // If context has user, use it
        if (auth?.user?.id) {
            console.log('[CreateTeamPage] auth context user present', auth.user.id);
            setOwnerId(auth.user.id);
            return;
        }
        console.log('[CreateTeamPage] token present?', !!token, 'API_URL=', (import.meta.env.VITE_API_URL ?? 'default'));
        try {
            console.log('[CreateTeamPage] calling api.get("/auth/me") to resolve current user');
            const data = await api.get<{ id: string }>('/auth/me');
            console.log('[CreateTeamPage] /api/auth/me data', data);
            if (data?.id) setOwnerId(data.id as string);
            else setMessage('Authenticated but no user id returned');
        } catch (err) {
            console.error('[CreateTeamPage] loadMe error', err);
            setMessage('Error fetching current user');
        }
    }

    // Run on mount and when auth.user changes
    React.useEffect(() => {
        loadMe();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [auth?.user?.id]);

    function onFileChange(e: React.ChangeEvent<HTMLInputElement>) {
        const f = e.target.files && e.target.files[0];
        setLogo(f ?? null);
        console.log('[CreateTeamPage] selected file', f);
    }

    async function onSubmit(e: React.FormEvent) {
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
            console.log('[CreateTeamPage] sending POST /team via api.post');
            const created = await api.post('/team', fd as unknown as FormData);
            console.log('[CreateTeamPage] created team', created);
            setMessage('Team created successfully');
        } catch (err) {
            console.error('[CreateTeamPage] submit error', err);
            setMessage(`Network or unexpected error: ${String(err)}`);
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