import { LogOut } from "lucide-react";
import { useAuthStore } from "@/features/auth/model/auth-store";
import { authApi } from "@/features/auth/api/auth-api";
import { Button } from "@/shared/ui/button/Button";

export function Navbar() {
  const user = useAuthStore((state) => state.user);
  const clearSession = useAuthStore((state) => state.clearSession);

  const handleLogout = async () => {
    await authApi.logout().catch(() => null);
    clearSession();
  };

  return (
    <header className="flex h-14 items-center justify-between border-b border-slate-200 bg-white px-6">
      <div className="text-sm text-slate-500">
        {user ? `Halo, ${user.name}` : ""}
      </div>
      <Button variant="ghost" onClick={handleLogout}>
        <LogOut className="mr-2 h-4 w-4" />
        Keluar
      </Button>
    </header>
  );
}
