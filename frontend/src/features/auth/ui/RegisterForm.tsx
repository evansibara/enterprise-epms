import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { authApi } from "@/features/auth/api/auth-api";
import { Input } from "@/shared/ui/input/Input";
import { Button } from "@/shared/ui/button/Button";

const registerSchema = z
  .object({
    name: z.string().min(1, "Nama wajib diisi").max(150, "Nama maksimal 150 karakter"),
    email: z.string().email("Email tidak valid"),
    password: z
      .string()
      .min(8, "Password minimal 8 karakter")
      .regex(/[A-Z]/, "Password harus mengandung minimal 1 huruf besar")
      .regex(/[0-9]/, "Password harus mengandung minimal 1 angka"),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Konfirmasi password tidak cocok",
    path: ["confirmPassword"],
  });

type RegisterFormValues = z.infer<typeof registerSchema>;

export function RegisterForm() {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormValues>({ resolver: zodResolver(registerSchema) });

  const registerMutation = useMutation({
    mutationFn: (values: RegisterFormValues) =>
      authApi.register({
        name: values.name,
        email: values.email,
        password: values.password,
      }),
    onSuccess: () => {
      navigate("/login", {
        state: { registered: true },
      });
    },
  });

  return (
    <form
      onSubmit={handleSubmit((values) => registerMutation.mutate(values))}
      className="space-y-4"
    >
      <div>
        <label className="mb-1 block text-sm font-medium text-slate-700">
          Nama Lengkap
        </label>
        <Input type="text" placeholder="Nama Anda" {...register("name")} />
        {errors.name && (
          <p className="mt-1 text-xs text-red-600">{errors.name.message}</p>
        )}
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium text-slate-700">
          Email
        </label>
        <Input type="email" placeholder="you@company.com" {...register("email")} />
        {errors.email && (
          <p className="mt-1 text-xs text-red-600">{errors.email.message}</p>
        )}
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium text-slate-700">
          Password
        </label>
        <Input type="password" placeholder="••••••••" {...register("password")} />
        {errors.password && (
          <p className="mt-1 text-xs text-red-600">{errors.password.message}</p>
        )}
        <p className="mt-1 text-xs text-slate-400">
          Minimal 8 karakter, mengandung huruf besar dan angka.
        </p>
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium text-slate-700">
          Konfirmasi Password
        </label>
        <Input type="password" placeholder="••••••••" {...register("confirmPassword")} />
        {errors.confirmPassword && (
          <p className="mt-1 text-xs text-red-600">{errors.confirmPassword.message}</p>
        )}
      </div>

      {registerMutation.isError && (
        <p className="text-sm text-red-600">
          Registrasi gagal. Email mungkin sudah terdaftar, atau periksa kembali data Anda.
        </p>
      )}

      <Button type="submit" className="w-full" disabled={registerMutation.isPending}>
        {registerMutation.isPending ? "Memproses..." : "Daftar"}
      </Button>
    </form>
  );
}