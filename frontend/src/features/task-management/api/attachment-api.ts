import { apiClient } from "@/shared/api/axios-instance";
import type { Attachment } from "@/entities/attachment/attachment.types";

export const attachmentApi = {
  listByTask: (taskId: string) =>
    apiClient
      .get<Attachment[]>(`/tasks/${taskId}/attachments`)
      .then((res) => res.data),

  upload: (taskId: string, file: File) => {
    const formData = new FormData();
    formData.append("file", file);

    return apiClient
      .post<Attachment>(`/tasks/${taskId}/attachments`, formData, {
        headers: { "Content-Type": "multipart/form-data" },
      })
      .then((res) => res.data);
  },

  // Backend mengembalikan file stream langsung (FileContentResult), bukan JSON,
  // jadi kita minta blob lalu trigger download via <a> sementara di memori.
  download: async (attachmentId: string, fileName: string) => {
    const response = await apiClient.get(`/attachments/${attachmentId}/download`, {
      responseType: "blob",
    });

    const url = URL.createObjectURL(response.data as Blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  },

  remove: (attachmentId: string) => apiClient.delete(`/attachments/${attachmentId}`),
};
