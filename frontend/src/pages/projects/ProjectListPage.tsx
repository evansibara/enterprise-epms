import { useState, type MouseEvent } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { Plus, Pencil, ChevronLeft, ChevronRight } from "lucide-react";
import { AppLayout } from "@/shared/ui/AppLayout";
import { Card } from "@/shared/ui/card/Card";
import { Badge } from "@/shared/ui/badge/Badge";
import { Input } from "@/shared/ui/input/Input";
import { Button } from "@/shared/ui/button/Button";
import { useDebounce } from "@/shared/lib/use-debounce";
import { useAuthStore } from "@/features/auth/model/auth-store";
import { projectApi } from "@/features/project-management/api/project-api";
import { ProjectFormModal } from "@/features/project-management/ui/ProjectFormModal";
import type { PagedResult } from "@/features/project-management/api/project-api";
import type { Project, ProjectStatus } from "@/entities/project/project.types";

const PAGE_SIZE = 9;

const statusOptions: { value: ProjectStatus | ""; label: string }[] = [
  { value: "", label: "Semua Status" },
  { value: "Planning", label: "Perencanaan" },
  { value: "Active", label: "Aktif" },
  { value: "OnHold", label: "Ditahan" },
  { value: "Completed", label: "Selesai" },
  { value: "Cancelled", label: "Dibatalkan" },
];

export function ProjectListPage() {
  const user = useAuthStore((state) => state.user);
  const canManageProjects = user?.role === "Admin" || user?.role === "Manager";

  const [searchInput, setSearchInput] = useState("");
  const [status, setStatus] = useState<ProjectStatus | "">("");
  const [page, setPage] = useState(1);

  const [formModalOpen, setFormModalOpen] = useState(false);
  const [editingProject, setEditingProject] = useState<Project | null>(null);

  // Search di-debounce supaya tidak fetch ke API di setiap keystroke.
  const debouncedSearch = useDebounce(searchInput, 400);

  const { data, isLoading } = useQuery<PagedResult<Project>>({
    queryKey: ["projects", debouncedSearch, status, page],
    queryFn: () =>
      projectApi.list({
        search: debouncedSearch || undefined,
        status: status || undefined,
        page,
        pageSize: PAGE_SIZE,
      }),
  });

  const projects = data?.items ?? [];

  const openCreateModal = () => {
    setEditingProject(null);
    setFormModalOpen(true);
  };

  const openEditModal = (project: Project, event: MouseEvent) => {
    event.preventDefault(); // jangan ikut navigasi ke detail project
    event.stopPropagation();
    setEditingProject(project);
    setFormModalOpen(true);
  };

  return (
    <AppLayout>
      <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <h1 className="text-2xl font-semibold text-slate-900">Projects</h1>

        {canManageProjects && (
          <Button onClick={openCreateModal}>
            <Plus className="mr-2 h-4 w-4" />
            Buat Project
          </Button>
        )}
      </div>

      <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center">
        <Input
          placeholder="Cari proyek..."
          className="sm:max-w-xs"
          value={searchInput}
          onChange={(event) => {
            setSearchInput(event.target.value);
            setPage(1);
          }}
        />

        <select
          value={status}
          onChange={(event) => {
            setStatus(event.target.value as ProjectStatus | "");
            setPage(1);
          }}
          className="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-700 outline-none focus:border-primary-500 focus:ring-2 focus:ring-primary-100 sm:max-w-xs"
        >
          {statusOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </div>

      {isLoading && <p className="text-sm text-slate-500">Memuat proyek...</p>}

      {!isLoading && projects.length === 0 && (
        <p className="text-sm text-slate-400">Tidak ada project yang cocok.</p>
      )}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {projects.map((project) => (
          <Link key={project.id} to={`/projects/${project.id}`}>
            <Card className="h-full transition-shadow hover:shadow-md">
              <div className="mb-2 flex items-center justify-between gap-2">
                <h3 className="font-medium text-slate-900">{project.name}</h3>
                <div className="flex items-center gap-1">
                  <Badge tone="info">{project.status}</Badge>
                  {canManageProjects && (
                    <button
                      type="button"
                      onClick={(event) => openEditModal(project, event)}
                      className="rounded-md p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-600"
                      aria-label="Edit project"
                    >
                      <Pencil className="h-3.5 w-3.5" />
                    </button>
                  )}
                </div>
              </div>
              <p className="text-sm text-slate-500 line-clamp-2">
                {project.description}
              </p>
            </Card>
          </Link>
        ))}
      </div>

      {data && data.totalPages > 1 && (
        <div className="mt-6 flex items-center justify-center gap-2">
          <Button
            variant="secondary"
            disabled={!data.hasPreviousPage}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>

          <span className="text-sm text-slate-600">
            Halaman {data.page} dari {data.totalPages}
          </span>

          <Button
            variant="secondary"
            disabled={!data.hasNextPage}
            onClick={() => setPage((p) => p + 1)}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      )}

      <ProjectFormModal
        open={formModalOpen}
        onClose={() => setFormModalOpen(false)}
        project={editingProject}
      />
    </AppLayout>
  );
}
