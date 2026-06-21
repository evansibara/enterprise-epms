import { useParams, Link } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { AppLayout } from "@/shared/ui/AppLayout";
import { Card } from "@/shared/ui/card/Card";
import { Button } from "@/shared/ui/button/Button";
import { Badge } from "@/shared/ui/badge/Badge";
import { projectApi } from "@/features/project-management/api/project-api";
import { activityLogApi } from "@/features/project-management/api/activity-log-api";
import type { ProjectStatus } from "@/entities/project/project.types";

const statusTone: Record<ProjectStatus, "neutral" | "success" | "warning" | "danger" | "info"> = {
  Planning: "neutral",
  Active: "info",
  OnHold: "warning",
  Completed: "success",
  Cancelled: "danger",
};

const statusLabel: Record<ProjectStatus, string> = {
  Planning: "Perencanaan",
  Active: "Aktif",
  OnHold: "Ditahan",
  Completed: "Selesai",
  Cancelled: "Dibatalkan",
};

function formatDate(isoString: string | null): string {
  if (!isoString) return "Tidak ada tenggat waktu";
  return new Date(isoString).toLocaleDateString("id-ID", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
}

function formatDateTime(isoString: string): string {
  return new Date(isoString).toLocaleString("id-ID", {
    day: "numeric",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function describeAction(action: string, metadataJson: string | null): string {
  switch (action) {
    case "Created":
      return "membuat project ini";
    case "Updated":
      return "memperbarui detail project";
    case "Deleted":
      return "menghapus project ini";
    default: {
      if (metadataJson) {
        try {
          const meta = JSON.parse(metadataJson);
          if (meta.previousStatus && meta.newStatus) {
            return `mengubah status dari ${meta.previousStatus} ke ${meta.newStatus}`;
          }
        } catch {
          // metadataJson tidak terparse, jatuh ke fallback di bawah.
        }
      }
      return action;
    }
  }
}

export function ProjectDetailPage() {
  const { projectId } = useParams<{ projectId: string }>();

  const { data: project, isLoading: isProjectLoading } = useQuery({
    queryKey: ["projects", projectId],
    queryFn: () => projectApi.getById(projectId!),
    enabled: Boolean(projectId),
  });

  const { data: activityLogs, isLoading: isLogsLoading } = useQuery({
    queryKey: ["activity-logs", "project", projectId],
    queryFn: () => activityLogApi.listByProject(projectId!),
    enabled: Boolean(projectId),
  });

  return (
    <AppLayout>
      {isProjectLoading && <p className="text-sm text-slate-500">Memuat proyek...</p>}

      {project && (
        <>
          <div className="mb-6 flex items-center justify-between">
            <div>
              <div className="mb-1 flex items-center gap-2">
                <h1 className="text-2xl font-semibold text-slate-900">
                  {project.name}
                </h1>
                <Badge tone={statusTone[project.status]}>
                  {statusLabel[project.status]}
                </Badge>
              </div>
              <p className="text-sm text-slate-500">{project.description}</p>
            </div>
            <Link to={`/projects/${project.id}/board`}>
              <Button>Buka Kanban Board</Button>
            </Link>
          </div>

          <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
            <Card className="lg:col-span-1">
              <h2 className="mb-3 text-sm font-semibold text-slate-700">
                Detail Project
              </h2>
              <dl className="space-y-3 text-sm">
                <div>
                  <dt className="text-slate-400">Pemilik (Owner)</dt>
                  <dd className="text-slate-900">{project.ownerName ?? "—"}</dd>
                </div>
                <div>
                  <dt className="text-slate-400">Tenggat Waktu</dt>
                  <dd className="text-slate-900">{formatDate(project.deadline)}</dd>
                </div>
                <div>
                  <dt className="text-slate-400">Status</dt>
                  <dd className="text-slate-900">{statusLabel[project.status]}</dd>
                </div>
              </dl>
            </Card>

            <Card className="lg:col-span-2">
              <h2 className="mb-3 text-sm font-semibold text-slate-700">
                Riwayat Aktivitas
              </h2>

              {isLogsLoading && (
                <p className="text-sm text-slate-500">Memuat riwayat...</p>
              )}

              {activityLogs && activityLogs.length === 0 && (
                <p className="text-sm text-slate-400">Belum ada aktivitas tercatat.</p>
              )}

              {activityLogs && activityLogs.length > 0 && (
                <ul className="space-y-3">
                  {activityLogs.map((log) => (
                    <li key={log.id} className="border-b border-slate-100 pb-2 text-sm last:border-0">
                      <p className="text-slate-700">
                        <span className="font-medium text-slate-900">
                          {log.performedByUserName ?? "Seseorang"}
                        </span>{" "}
                        {describeAction(log.action, log.metadataJson)}
                      </p>
                      <p className="text-xs text-slate-400">
                        {formatDateTime(log.timestamp)}
                      </p>
                    </li>
                  ))}
                </ul>
              )}
            </Card>
          </div>
        </>
      )}
    </AppLayout>
  );
}