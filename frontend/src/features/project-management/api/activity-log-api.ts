import { apiClient } from "@/shared/api/axios-instance";

// Mencerminkan EPMS.Application.DTOs.ActivityLogs.ActivityLogResponseDto.
export interface ActivityLog {
  id: string;
  entityType: string;
  entityId: string;
  action: string;
  performedByUserId: string;
  performedByUserName: string | null;
  timestamp: string;
  metadataJson: string | null;
}

export const activityLogApi = {
  listByProject: (projectId: string) =>
    apiClient
      .get<ActivityLog[]>(`/projects/${projectId}/activity-logs`)
      .then((res) => res.data),

  listByTask: (taskId: string) =>
    apiClient
      .get<ActivityLog[]>(`/tasks/${taskId}/activity-logs`)
      .then((res) => res.data),
};