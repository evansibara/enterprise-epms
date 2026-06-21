import { apiClient } from "@/shared/api/axios-instance";
import type { Project } from "@/entities/project/project.types";
import type { PagedResult } from "@/shared/api/paged-result";

// Re-export supaya kode lama yang mengimpor PagedResult dari sini
// (mis. ProjectListPage.tsx) tidak perlu diubah.
export type { PagedResult } from "@/shared/api/paged-result";

export interface ProjectListParams {
  search?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}

export const projectApi = {
  list: (params?: ProjectListParams) =>
    apiClient
      .get<PagedResult<Project>>("/projects", { params })
      .then((res) => res.data),

  getById: (id: string) =>
    apiClient.get<Project>(`/projects/${id}`).then((res) => res.data),

  create: (payload: Pick<Project, "name" | "description" | "deadline">) =>
    apiClient.post<Project>("/projects", payload).then((res) => res.data),

  // Backend: PUT /api/v1/projects/{id} (ProjectsController.Update),
  // bukan PATCH — dan payload wajib Name + Status terisi (lihat
  // UpdateProjectRequestDto), bukan Partial<Project> bebas.
  update: (id: string, payload: Pick<Project, "name" | "description" | "deadline" | "status">) =>
    apiClient.put<Project>(`/projects/${id}`, payload).then((res) => res.data),

  remove: (id: string) => apiClient.delete(`/projects/${id}`), // soft delete di backend
};