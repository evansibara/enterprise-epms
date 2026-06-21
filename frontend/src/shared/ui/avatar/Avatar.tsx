import { cn } from "@/shared/lib/cn";

interface AvatarProps {
  name?: string | null;
  className?: string;
}

const palette = [
  "bg-blue-100 text-blue-700",
  "bg-emerald-100 text-emerald-700",
  "bg-amber-100 text-amber-700",
  "bg-violet-100 text-violet-700",
  "bg-rose-100 text-rose-700",
  "bg-cyan-100 text-cyan-700",
];

const DEFAULT_COLOR = palette[0] as string;

function colorFor(name: string): string {
  const sum = name.split("").reduce((acc, ch) => acc + ch.charCodeAt(0), 0);
  return palette[sum % palette.length] ?? DEFAULT_COLOR;
}

function initialsFor(name: string): string {
  const parts = name.trim().split(/\s+/);
  const first = parts[0]?.[0] ?? "";
  const last = parts.length > 1 ? parts[parts.length - 1]?.[0] ?? "" : "";
  return (first + last).toUpperCase();
}

/**
 * Avatar berbasis inisial nama. Backend (UserResponseDto) belum punya field
 * `avatarUrl`, jadi belum ada gambar foto profil sungguhan untuk ditampilkan —
 * ini representasi visual sementara yang tetap memenuhi spesifikasi
 * "Assignee Avatar" di Task Card tanpa butuh upload foto profil.
 */
export function Avatar({ name, className }: AvatarProps) {
  if (!name) {
    return (
      <div
        className={cn(
          "flex h-6 w-6 items-center justify-center rounded-full bg-slate-100 text-[10px] font-medium text-slate-400",
          className,
        )}
        title="Belum ditugaskan"
      >
        —
      </div>
    );
  }

  const safeName: string = name;

  return (
    <div
      className={cn(
        "flex h-6 w-6 items-center justify-center rounded-full text-[10px] font-semibold",
        colorFor(safeName),
        className,
      )}
      title={safeName}
    >
      {initialsFor(safeName)}
    </div>
  );
}