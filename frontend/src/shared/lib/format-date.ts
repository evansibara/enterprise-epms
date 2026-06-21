/** Format tanggal lengkap, mis. "21 Juni 2026". */
export function formatDate(isoString: string | null | undefined): string {
  if (!isoString) return "—";
  return new Date(isoString).toLocaleDateString("id-ID", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
}

/** Format tanggal + jam, mis. "21 Jun 2026, 14.30". */
export function formatDateTime(isoString: string | null | undefined): string {
  if (!isoString) return "—";
  return new Date(isoString).toLocaleString("id-ID", {
    day: "numeric",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

/**
 * Format relatif, mis. "2 jam lalu", dipakai di timeline Activity Log & Comment.
 * Jatuh balik ke formatDateTime untuk rentang yang sudah lama (>30 hari).
 */
export function formatRelativeTime(isoString: string | null | undefined): string {
  if (!isoString) return "—";

  const date = new Date(isoString);
  const diffMs = Date.now() - date.getTime();
  const diffSeconds = Math.round(diffMs / 1000);

  if (diffSeconds < 60) return "Baru saja";

  const diffMinutes = Math.round(diffSeconds / 60);
  if (diffMinutes < 60) return `${diffMinutes} menit lalu`;

  const diffHours = Math.round(diffMinutes / 60);
  if (diffHours < 24) return `${diffHours} jam lalu`;

  const diffDays = Math.round(diffHours / 24);
  if (diffDays < 30) return `${diffDays} hari lalu`;

  return formatDateTime(isoString);
}

/** True jika due date sudah lewat atau dalam <=3 hari ke depan (dipakai highlight merah di Kanban). */
export function isDueSoonOrOverdue(dueDate: string | null | undefined): boolean {
  if (!dueDate) return false;
  const threshold = Date.now() + 3 * 24 * 60 * 60 * 1000;
  return new Date(dueDate).getTime() <= threshold;
}
