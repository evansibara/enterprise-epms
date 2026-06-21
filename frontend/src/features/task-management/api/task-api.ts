import { apiClient } from "@/shared/api/axios-instance";
import type { Task, TaskStatus } from "@/entities/task/task.types";

export const taskApi = {
  listByProject: (projectId: string) =>
    apiClient
      .get<Task[]>(`/projects/${projectId}/tasks`)
      .then((res) => res.data),

  updateStatus: (taskId: string, status: TaskStatus) =>
    apiClient
      .patch<Task>(`/tasks/${taskId}/status`, { status })
      .then((res) => res.data),

  create: (projectId: string, payload: Pick<Task, "title" | "description" | "priority">) =>
    apiClient
      .post<Task>(`/projects/${projectId}/tasks`, payload)
      .then((res) => res.data),
};