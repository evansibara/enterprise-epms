import { apiClient } from "@/shared/api/axios-instance";
import type { Project } from "@/entities/project/project.types";

export interface ProjectListParams {
  search?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}

// Mencerminkan EPMS.Application.DTOs.Common.PagedResult<T> dari backend.
// GET /api/v1/projects TIDAK mengembalikan array langsung, melainkan
// objek paging ini — jangan diubah jadi Project[] tanpa mengubah backend.
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
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