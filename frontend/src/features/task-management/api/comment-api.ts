import { apiClient } from "@/shared/api/axios-instance";
import type { Comment } from "@/entities/comment/comment.types";

export const commentApi = {
  listByTask: (taskId: string) =>
    apiClient.get<Comment[]>(`/tasks/${taskId}/comments`).then((res) => res.data),

  create: (taskId: string, content: string) =>
    apiClient
      .post<Comment>(`/tasks/${taskId}/comments`, { content })
      .then((res) => res.data),

  remove: (commentId: string) => apiClient.delete(`/comments/${commentId}`),
};
