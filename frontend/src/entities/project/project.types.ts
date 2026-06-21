export type ProjectStatus = "Planning" | "Active" | "OnHold" | "Completed" | "Cancelled";

export interface Project {
  id: string;
  name: string;
  description: string;
  deadline: string | null;
  status: ProjectStatus;
  ownerId: string;
  ownerName: string | null;
  createdAt: string;
}