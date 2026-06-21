import { useNavigate } from "react-router-dom";
import { LogOut } from "lucide-react";
import { AppLayout } from "@/shared/ui/AppLayout";
import { Card } from "@/shared/ui/card/Card";
import { Badge } from "@/shared/ui/badge/Badge";
import { Button } from "@/shared/ui/button/Button";
import { Avatar } from "@/shared/ui/avatar/Avatar";
import { useAuthStore } from "@/features/auth/model/auth-store";
import { authApi } from "@/features/auth/api/auth-api";

export function SettingsPage() {
  const navigate = useNavigate();
  const user = useAuthStore((state) => state.user);
  const clearSession = useAuthStore((state) => state.clearSession);

  const handleLogout = async () => {
    try {
      await authApi.logout();
    } finally {
      clearSession();
      navigate("/login");
    }
  };

  return (
    <AppLayout>
      <h1 className="mb-6 text-2xl font-semibold text-slate-900">Settings</h1>

      <Card className="max-w-md">
        <div className="flex items-center gap-3">
          <Avatar name={user?.name} className="h-12 w-12 text-base" />
          <div>
            <p className="font-medium text-slate-900">{user?.name}</p>
            <p className="text-sm text-slate-500">{user?.email}</p>
          </div>
        </div>

        <div className="mt-4 flex items-center justify-between border-t border-slate-100 pt-4">
          <span className="text-sm text-slate-500">Role</span>
          <Badge tone="info">{user?.role}</Badge>
        </div>

        <p className="mt-4 text-xs text-slate-400">
          Pengaturan profil (ubah nama/password) belum tersedia — perlu
          endpoint tambahan di backend untuk fitur ini.
        </p>

        <Button variant="secondary" className="mt-6 w-full" onClick={handleLogout}>
          <LogOut className="mr-2 h-4 w-4" />
          Keluar
        </Button>
      </Card>
    </AppLayout>
  );
}
