import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { AppLayout } from "@/shared/ui/AppLayout";
import { Card } from "@/shared/ui/card/Card";
import { Badge } from "@/shared/ui/badge/Badge";
import { Input } from "@/shared/ui/input/Input";
import { projectApi } from "@/features/project-management/api/project-api";
import type { PagedResult } from "@/features/project-management/api/project-api";
import type { Project } from "@/entities/project/project.types";

/**
 * TODO: hubungkan search & filter ke query params endpoint
 * GET /projects?search=&status=&page=&pageSize= (section 4.7 spesifikasi).
 */
export function ProjectListPage() {
  const { data, isLoading } = useQuery<PagedResult<Project>>({
    queryKey: ["projects"],
    queryFn: () => projectApi.list(),
  });
  
  const projects = data?.items;

  return (
    <AppLayout>
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-slate-900">Projects</h1>
        <Input placeholder="Cari proyek..." className="max-w-xs" />
      </div>

      {isLoading && <p className="text-sm text-slate-500">Memuat proyek...</p>}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {projects?.map((project) => (
          <Link key={project.id} to={`/projects/${project.id}`}>
            <Card className="h-full transition-shadow hover:shadow-md">
              <div className="mb-2 flex items-center justify-between">
                <h3 className="font-medium text-slate-900">{project.name}</h3>
                <Badge tone="info">{project.status}</Badge>
              </div>
              <p className="text-sm text-slate-500 line-clamp-2">
                {project.description}
              </p>
            </Card>
          </Link>
        ))}
      </div>
    </AppLayout>
  );
}