import { Link, useLocation } from "react-router-dom";
import { LoginForm } from "@/features/auth/ui/LoginForm";
import { Card } from "@/shared/ui/card/Card";

export function LoginPage() {
  const location = useLocation();
  const justRegistered = Boolean(
    (location.state as { registered?: boolean } | null)?.registered,
  );

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
      <Card className="w-full max-w-sm">
        <h1 className="mb-1 text-xl font-semibold text-slate-900">
          Masuk ke EPMS
        </h1>
        <p className="mb-6 text-sm text-slate-500">
          Enterprise Project Management System
        </p>

        {justRegistered && (
          <p className="mb-4 rounded-md bg-green-50 px-3 py-2 text-sm text-green-700">
            Registrasi berhasil! Silakan masuk dengan akun baru Anda.
          </p>
        )}

        <LoginForm />

        <p className="mt-4 text-center text-sm text-slate-500">
          Belum punya akun?{" "}
          <Link to="/register" className="font-medium text-primary-600">
            Daftar
          </Link>
        </p>
      </Card>
    </div>
  );
}