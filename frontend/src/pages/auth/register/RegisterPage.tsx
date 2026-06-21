import { Link } from "react-router-dom";
import { Card } from "@/shared/ui/card/Card";
import { RegisterForm } from "@/features/auth/ui/RegisterForm";

export function RegisterPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
      <Card className="w-full max-w-sm">
        <h1 className="mb-1 text-xl font-semibold text-slate-900">
          Buat Akun EPMS
        </h1>
        <p className="mb-6 text-sm text-slate-500">
          Enterprise Project Management System
        </p>

        <RegisterForm />

        <p className="mt-4 text-center text-sm text-slate-500">
          Sudah punya akun?{" "}
          <Link to="/login" className="font-medium text-primary-600">
            Masuk
          </Link>
        </p>
      </Card>
    </div>
  );
}