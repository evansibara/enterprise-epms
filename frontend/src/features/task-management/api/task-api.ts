import { apiClient } from "@/shared/api/axios-instance";
import type { Task, TaskPriority, TaskStatus } from "@/entities/task/task.types";

export interface CreateTaskPayload {
  title: string;
  description?: string;
  assigneeId?: string | null;
  priority: TaskPriority;
  dueDate?: string | null;
}

export interface UpdateTaskPayload {
  title: string;
  description?: string;
  priority: TaskPriority;
  dueDate?: string | null;
}

export const taskApi = {
  listByProject: (projectId: string) =>
    apiClient
      .get<Task[]>(`/projects/${projectId}/tasks`)
      .then((res) => res.data),

  getById: (taskId: string) =>
    apiClient.get<Task>(`/tasks/${taskId}`).then((res) => res.data),

  create: (projectId: string, payload: CreateTaskPayload) =>
    apiClient
      .post<Task>(`/projects/${projectId}/tasks`, payload)
      .then((res) => res.data),

  update: (taskId: string, payload: UpdateTaskPayload) =>
    apiClient.put<Task>(`/tasks/${taskId}`, payload).then((res) => res.data),

  updateStatus: (taskId: string, status: TaskStatus) =>
    apiClient
      .patch<Task>(`/tasks/${taskId}/status`, { status })
      .then((res) => res.data),

  assign: (taskId: string, assigneeId: string) =>
    apiClient
      .patch<Task>(`/tasks/${taskId}/assign`, { assigneeId })
      .then((res) => res.data),

  remove: (taskId: string) => apiClient.delete(`/tasks/${taskId}`),
};
