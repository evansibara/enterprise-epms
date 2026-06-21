import { apiClient } from "@/shared/api/axios-instance";

// Mencerminkan EPMS.Application.DTOs.Dashboard.DashboardSummaryDto.
export interface DashboardSummary {
  totalProjects: number;
  activeTasks: number;
  completedTasks: number;
  teamMembers: number;
  pendingDeadlines: number;
}

export const dashboardApi = {
  getSummary: () =>
    apiClient.get<DashboardSummary>("/dashboard/summary").then((res) => res.data),
};