import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "@/features/auth/model/auth-store";
import type { UserRole } from "@/entities/user/user.types";

interface ProtectedRouteProps {
  allowedRoles: UserRole[];
}

/**
 * Guard berbasis role sesuai spesifikasi RBAC:
 * Admin > Manager > Employee.
 * Bungkus <Route> dengan elemen ini, anak-anaknya hanya
 * bisa diakses jika role user termasuk dalam allowedRoles.
 */
export function ProtectedRoute({ allowedRoles }: ProtectedRouteProps) {
  const user = useAuthStore((state) => state.user);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

  if (!isAuthenticated || !user) {
    return <Navigate to="/login" replace />;
  }

  if (!allowedRoles.includes(user.role)) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
