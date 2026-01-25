import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";

export default function RequireUnauth({ children }: { children: ReactNode }) {
    const token = localStorage.getItem("mf_token");

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    return children;
}