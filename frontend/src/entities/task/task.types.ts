export type TaskStatus = "ToDo" | "InProgress" | "Review" | "Done";
export type TaskPriority = "Low" | "Medium" | "High" | "Urgent";
 
export interface Task {
  id: string;
  projectId: string;
  title: string;
  description: string;
  assigneeId: string | null;
  assigneeName?: string | null;
  priority: TaskPriority;
  status: TaskStatus;
  dueDate: string | null;
  createdAt: string;
  deletedAt: string | null;
}